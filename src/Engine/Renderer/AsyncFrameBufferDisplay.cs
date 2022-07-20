using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

using System.IO.MemoryMappedFiles;
using System.Diagnostics;

using GameHaven.Engine.Diagnostics;

namespace GameHaven.Engine.Renderer {

    /**
       Note: For now this class encapsulates everything for writing to /dev/fb*, and presumes
                that the display is set to 16bpp 5/6/5 (RGB) mode and 800x480 resolution. 
     **/
    public class AsyncFrameBufferDisplay : IDisposable {
        private ILogger _logger;

        private readonly int _width = 800;
        private readonly int _height = 480;
        private readonly int _bpp = 16;  // RBG565
        private readonly string _fbDevice = "/dev/fb0";

        private MemoryMappedFile? _fbMmFile = null;
        private MemoryMappedViewStream? _fbStream = null;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _renderLoopTask;
        
        private object _mutex = new object();
        private byte[]? _nextItem;        

        protected bool _disposed = false;

        private long _lastFrameTime = 0;
        
        public AsyncFrameBufferDisplay() {            
            _logger = Logging.GetLogger("AsyncFrameBufferDisplay");
            _renderLoopTask = Task.Factory.StartNew(() => RenderLoop(), TaskCreationOptions.LongRunning);
        }

        public void Clear() {
            _logger.LogDebug("Clear()!");
            using(Image<Bgr565> image = new Image<Bgr565>(_width, _height, new Bgr565(1,1,1))) {
                DisplayImage(image);
            }        
        }

        public void DisplayImage(string filename) {
            _logger.LogDebug($"DisplayImage({filename})");
            using(Image<Bgr565> image = Image.Load<Bgr565>(filename)) {                
                if (image.Width < _width || image.Width > _width) {
                    image.Mutate(x =>
                    {
                        x.Resize(_width, _height);
                    });
                }
                DisplayImage(image);
            }            
        }
        
        protected void RenderToDisplay(byte[] item)
        {    
            if (EnsureOpenStream()) {                
                _fbStream?.Seek(0, SeekOrigin.Begin);
                _logger.LogDebug($"Writing to FB - Stream pos: {_fbStream?.Position} Length: {_fbStream?.Length} Buffer Size: {item.Length}");
                _fbStream?.Write(item, 0, (int)_fbStream.Length);
                _fbStream?.Flush();                
            }            
        }       

        private void DisplayImage(Image<Bgr565> image) {
            byte[] pixelBytes = new byte[image.Width * image.Height * (_bpp / 8)];
            image.CopyPixelDataTo(pixelBytes);
            lock(_mutex) {
                // Last one in wins
                _nextItem = pixelBytes;
            }            
        }

        private bool EnsureOpenStream() {
            try {
                if (_fbMmFile == null) {        
                    _fbMmFile = MemoryMappedFile.CreateFromFile(_fbDevice, FileMode.Open, null, (_width * _height * (_bpp / 8)));
                    _fbStream = _fbMmFile.CreateViewStream();
                }

                return true;
            } catch(Exception ex) {
                _logger.LogError($"Unable to ensure framebuffer stream: ${ex}");
                return false;
            }
        }

        private long LastFrameTime {
            get {
                return Interlocked.Read(ref _lastFrameTime);
            }
        }

        private void RenderLoop() {
            Stopwatch timer = Stopwatch.StartNew();

            while(!_cancellationTokenSource.Token.IsCancellationRequested) {          
                byte[]? currentItem = null;
                lock(_mutex) {
                    // Grab while we're preventing changes, then release before rendering
                    currentItem = _nextItem;                    
                    _nextItem = null;
                }

                if (currentItem?.Length > 0) {
                    long frameStart = timer.ElapsedMilliseconds;
                    RenderToDisplay(currentItem);
                    long frameEnd = timer.ElapsedMilliseconds;
                    Interlocked.Exchange(ref _lastFrameTime, (frameEnd - frameStart));                    
                }

                Thread.Sleep(10);
            }
        }

        ~AsyncFrameBufferDisplay() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {            
            if (!_disposed) {
                if (disposing) {
                    _cancellationTokenSource.Cancel();
                    _renderLoopTask.Wait();

                    if (_fbStream != null) {
                        _fbStream.Dispose();
                        _fbStream = null;
                    }

                    if (_fbMmFile != null) {
                        _fbMmFile.Dispose();
                        _fbMmFile = null;
                    }
                }
                _disposed = true;
            }            
        }
    }
}