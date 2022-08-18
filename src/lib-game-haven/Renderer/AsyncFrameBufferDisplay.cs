using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

using Microsoft.Extensions.Logging;

using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Runtime.InteropServices;

using GameHaven.Diagnostics;

using Nfw.Linux.FrameBuffer;

namespace GameHaven.Renderer {
    public class AsyncFrameBufferDisplay : IDisposable {
        private ILogger _logger;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _renderLoopTask;
        
        private object _mutex = new object();
        private byte[]? _nextItem;        

        protected bool _disposed = false;

        private long _lastFrameTime = 0;
        private RawFrameBuffer _fb;

        private const string DEFAULT_DISPLAY_DEVICE = "/dev/fb0";

        public int RenderWidth { get; set; } = 800;
        public int RenderHeight { get; set; } = 480;
        public int RenderDepth { get; set; } = 16;  //  RBG565

        public int DisplayWidth { get { return _fb.PixelWidth; } }
        public int DisplayHeight { get { return _fb.PixelHeight; } }
        public int DisplayDepth { get { return _fb.PixelDepth; } }

        // Enumerates /dev/fb* and returns a display for the highest numbered display
        //  on the presumption that that is the correct one... could be config'd later
        //  etc perhaps? Throws if no fb's found
        public static AsyncFrameBufferDisplay CreateDisplay() {
            ILogger logger = Logging.GetLogger($"AsyncFrameBufferDisplay::CreateDisplay");
            string? firstFb = Directory.EnumerateFiles("/dev/", "fb?").OrderByDescending(x => x).FirstOrDefault();
            if (firstFb == null) {
                logger.LogError("/dev/fb? is empty?");
                throw new Exception("Unable to locate a framebuffer device in /dev/fb?");
            }

            logger.LogInformation($"Using framebuffer device at: {firstFb}");
            return new AsyncFrameBufferDisplay(firstFb);
        }
        
        public AsyncFrameBufferDisplay() : this(DEFAULT_DISPLAY_DEVICE) {
        }

        public AsyncFrameBufferDisplay(string device) {            
            _logger = Logging.GetLogger($"AsyncFrameBufferDisplay({device})");

            _fb = new RawFrameBuffer(device, _logger, true);
            _renderLoopTask = Task.Factory.StartNew(() => RenderLoop(), TaskCreationOptions.LongRunning);
        }

        public void Clear() {
            _logger.LogDebug("Clear()!");
            _fb.Clear();
        }

        public void DisplayImage(string filename) {
            _logger.LogDebug($"DisplayImage({filename})");
            using(Image<Bgr565> image = Image.Load<Bgr565>(filename)) {                                
                DisplayImage(image);
            }            
        }
        
        protected void RenderToDisplay(byte[] item) {                
            _fb.WriteRaw(item);
        }       

        private void DisplayImage(Image<Bgr565> image) {
            if (image.Width != _fb.PixelWidth || image.Height != _fb.PixelHeight) {
                image.Mutate(x =>
                {
                    x.Resize(_fb.PixelWidth, _fb.PixelHeight);
                });
            }

            byte[] pixelBytes = new byte[image.Width * image.Height * (_fb.PixelDepth / 8)];
            image.CopyPixelDataTo(pixelBytes);
            lock(_mutex) {
                // Last one in wins
                _nextItem = pixelBytes;
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
                    _fb.Dispose();
                }
                _disposed = true;
            }            
        }
    }
}