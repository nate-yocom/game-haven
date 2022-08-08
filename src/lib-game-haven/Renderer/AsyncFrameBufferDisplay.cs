using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

using Microsoft.Extensions.Logging;

using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Runtime.InteropServices;

using GameHaven.Diagnostics;

namespace GameHaven.Renderer {
    public class AsyncFrameBufferDisplay : IDisposable {
        private ILogger _logger;

        private readonly int _renderWidth = 800;
        private readonly int _renderHeight = 480;
        private readonly int _renderBpp = 16;  // RBG565
        private readonly string _fbDevice = "/dev/fb0";

        private int _displayWidth = 0;
        private int _displayHeight = 0;
        private int _displayBpp = 0;
        private string _displayName = "<unknown>";

        private MemoryMappedFile? _fbMmFile = null;
        private MemoryMappedViewStream? _fbStream = null;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _renderLoopTask;
        
        private object _mutex = new object();
        private byte[]? _nextItem;        

        protected bool _disposed = false;

        private long _lastFrameTime = 0;

        private const string DEFAULT_DISPLAY_DEVICE = "/dev/fb0";

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
            _fbDevice = device;
            _logger = Logging.GetLogger($"AsyncFrameBufferDisplay({_fbDevice})");

            // Should this go to an Initialize() so caller controls timing?
            ProbeFramebuffer();
            _renderLoopTask = Task.Factory.StartNew(() => RenderLoop(), TaskCreationOptions.LongRunning);
        }

        public void Clear() {
            _logger.LogDebug("Clear()!");
            using(Image<Bgr565> image = new Image<Bgr565>(_displayWidth, _displayHeight, new Bgr565(1,1,1))) {
                DisplayImage(image);
            }        
        }

        public void DisplayImage(string filename) {
            _logger.LogDebug($"DisplayImage({filename})");
            using(Image<Bgr565> image = Image.Load<Bgr565>(filename)) {                                
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
            if (image.Width != _displayWidth || image.Height != _displayHeight) {
                image.Mutate(x =>
                {
                    x.Resize(_displayWidth, _displayHeight);
                });
            }

            byte[] pixelBytes = new byte[image.Width * image.Height * (_renderBpp / 8)];
            image.CopyPixelDataTo(pixelBytes);
            lock(_mutex) {
                // Last one in wins
                _nextItem = pixelBytes;
            }            
        }

        private bool EnsureOpenStream() {
            try {
                if (_fbMmFile == null) {        
                    _fbMmFile = MemoryMappedFile.CreateFromFile(_fbDevice, FileMode.Open, null, (_displayWidth * _displayHeight * (_displayBpp / 8)));
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

        // FB api from https://www.kernel.org/doc/Documentation/fb/api.txt
        //  mapped for the bits we need, ignoring trailing parts (with byte[] only) for now

        [StructLayout(LayoutKind.Sequential)]
        private struct fb_fix_screeninfo {
            // char[] 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] 
            public byte[] id;

            // unsigned long 
            [MarshalAs(UnmanagedType.U4)] 
            public uint smem_start;

            // __u32
            [MarshalAs(UnmanagedType.U4)] 
            public uint smem_len;

            // __u32
            [MarshalAs(UnmanagedType.U4)] 
            public uint type;

            // Remaing bits we dont care about
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)] 
            public byte[] __remaining_bits;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct fb_var_screeninfo {
            public int xres;
            public int yres;
            public int xres_virtual;
            public int yres_virtual;
            public int xoffset;
            public int yoffset;
            public int bits_per_pixel;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 132)] 
            public byte[] __remaining_bits;
        };


        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int ioctl(int handle, uint request, ref fb_var_screeninfo capability);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int ioctl(int handle, uint request, ref fb_fix_screeninfo capability);
        
        [DllImport("libc", EntryPoint = "__errno_location")]
        private static extern System.IntPtr __errno_location();

        private const int FBIOGET_FSCREENINFO = 0x4602;
        private const int FBIOGET_VSCREENINFO = 0x4600;
                
        private void ProbeFramebuffer() {            
            using(var fileHandle = File.OpenHandle(_fbDevice, FileMode.Open, FileAccess.ReadWrite, FileShare.None, FileOptions.None, 0)) {
                fb_fix_screeninfo fixed_info = new fb_fix_screeninfo();
                fb_var_screeninfo variable_info = new fb_var_screeninfo();

                if(ioctl(fileHandle.DangerousGetHandle().ToInt32(), FBIOGET_FSCREENINFO, ref fixed_info) < 0) {
                    _logger.LogError($"ProbeFrameBuffer ioctl({FBIOGET_FSCREENINFO}) error: {System.Runtime.InteropServices.Marshal.ReadInt32(__errno_location())}");
                } else {
                    _displayName = System.Text.ASCIIEncoding.ASCII.GetString(fixed_info.id).TrimEnd(new char[] { '\r', '\n', ' ', '\0' });                    
                    _logger.LogDebug($"Display memory for {_displayName} starts at: {fixed_info.smem_start} length: {fixed_info.smem_len}");
                }

                if(ioctl(fileHandle.DangerousGetHandle().ToInt32(), FBIOGET_VSCREENINFO, ref variable_info) < 0) {
                    _logger.LogError($"ProbeFrameBuffer ioctl({FBIOGET_VSCREENINFO}) error: {System.Runtime.InteropServices.Marshal.ReadInt32(__errno_location())}");
                } else {
                    _logger.LogDebug($"Actual width => {variable_info.xres} height => {variable_info.yres} bpp => {variable_info.bits_per_pixel}");
                    _displayBpp = variable_info.bits_per_pixel;
                    _displayWidth = variable_info.xres;
                    _displayHeight = variable_info.yres;
                }       
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