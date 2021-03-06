﻿using MediaToolkit.Utils;
using MediaToolkit.NativeAPIs;
using SharpDX;
using SharpDX.Direct3D9;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GDI = System.Drawing;
using MediaToolkit.Logging;
using MediaToolkit.SharedTypes;

namespace MediaToolkit.ScreenCaptures
{


    /// <summary>
    /// с включенной композитной отрисовкой работает лучше чем GDI
    /// </summary>
    public class Direct3D9Capture : ScreenCapture
    {
       
        public Direct3D9Capture(object[] args) : base()
        {
            if(args!=null && args.Length > 0)
            {
                this.hWnd = (IntPtr)args[0];
            }

        }

        private Direct3D direct3D9 = new Direct3D();
        private Device device = null;
        private AdapterInformation adapterInfo = null;
        private PresentParameters presentParams = default(PresentParameters);

        private Surface srcSurface = null;
        private Surface destSurface = null;
        private Surface tmpSurface = null;

        private IntPtr hWnd = IntPtr.Zero;



        public override void Init(GDI.Rectangle srcRect, GDI.Size destSize)
        {
            logger.Debug("Direct3DCapture::Init(...)");

            base.Init(srcRect, destSize);

            //this.videoBuffer = new VideoBuffer(destSize.Width, destSize.Height, PixelFormat.Format32bppArgb);

            adapterInfo = direct3D9.Adapters.FirstOrDefault(); //  DefaultAdapter;//direct3D9.Adapters[1];

            var hMonitor = User32.GetMonitorFromRect(srcRect);
            if (hMonitor != IntPtr.Zero)
            {
                adapterInfo = direct3D9.Adapters.FirstOrDefault(a => a.Monitor == hMonitor);
            }
         
            if (hWnd == IntPtr.Zero)
            {// иначе не работает Device.Reset()
                hWnd = User32.GetDesktopWindow();
            }

            logger.Info("DefaultAdapter " + " " + adapterInfo.Details.DeviceName + " " + adapterInfo.Details.Description);

            var displayMode = adapterInfo.CurrentDisplayMode;
            logger.Info("CurrentDisplayMode " + " "  + displayMode.Width +"x" + displayMode.Height + " "+ displayMode.Format);

            //Rectangle clientRect = NativeMethods.GetAbsoluteClientRect(hWnd);

            presentParams = new PresentParameters
            {
                
                //BackBufferHeight = clientRect.Height,
                //BackBufferWidth = clientRect.Width,
                BackBufferHeight = adapterInfo.CurrentDisplayMode.Height,
                BackBufferWidth = adapterInfo.CurrentDisplayMode.Width,
                BackBufferFormat = adapterInfo.CurrentDisplayMode.Format,
                BackBufferCount = 1,

                Windowed = true,
                
                MultiSampleType = MultisampleType.None,
                SwapEffect = SwapEffect.Discard,

                PresentFlags = PresentFlags.None,
                PresentationInterval = PresentInterval.Default | PresentInterval.Immediate,

            };


            CreateFlags Flags = (CreateFlags.Multithreaded | CreateFlags.FpuPreserve | CreateFlags.HardwareVertexProcessing);

            //CreateFlags Flags = (CreateFlags.SoftwareVertexProcessing);

            device = new Device(direct3D9, adapterInfo.Adapter, DeviceType.Hardware, hWnd, Flags, presentParams);

            
            InitSurfaces();


        }

        private void InitSurfaces()
        {
            logger.Debug("InitSurfaces()");

            //AdapterInformation adapterInfo = direct3D9.Adapters.DefaultAdapter;
            var displayMode = adapterInfo.CurrentDisplayMode;

            /*
             * The buffer pointed will be filled with a representation of the front buffer, converted to the standard 32 bits per pixel format D3DFMT_A8R8G8B8.
             */
            srcSurface = Surface.CreateOffscreenPlain(device, displayMode.Width, displayMode.Height, Format.A8R8G8B8, Pool.SystemMemory);

            tmpSurface = Surface.CreateRenderTarget(device, displayMode.Width, displayMode.Height, Format.A8R8G8B8, MultisampleType.None, 0, true);
            //tmpSurface = Surface.CreateRenderTarget(device, displayMode.Width, displayMode.Height, Format.X8R8G8B8, MultisampleType.None, 0, true);

            destSurface = Surface.CreateRenderTarget(device, DestSize.Width, DestSize.Height, Format.A8R8G8B8, MultisampleType.None, 0, true);
            //destSurface = Surface.CreateRenderTarget(device, videoBuffer.bitmap.Width, videoBuffer.bitmap.Height, Format.X8R8G8B8, MultisampleType.None, 0, true);
        }

        private Stopwatch sw = new Stopwatch();

        public override ErrorCode UpdateBuffer(int timeout = 10)
        {
            sw.Restart();

            ErrorCode errorCode = ErrorCode.Unexpected;
            

            Result result = device.TestCooperativeLevel();
            if (result != ResultCode.Success)
            {
                logger.Warn("TestCooperativeLevel: " + result);
                bool deviceReady = false;
                if (result == ResultCode.DeviceLost)
                {
                    // OnLostDevice();
                    Thread.Sleep(50);
                }
                else if (result == ResultCode.DeviceNotReset)
                {
                    deviceReady = ReInitDevice();
                }
 
                if (!deviceReady)
                {
                    //TODO: error
                    logger.Warn("Device is not ready...");

                    Thread.Sleep(100);

                    return ErrorCode.NotReady;
                }

            }

            try
            {
                device.GetFrontBufferData(0, srcSurface);
                
                if (CaptureMouse)
                {
                    var hDc = srcSurface.GetDC();
                    User32.DrawCursorEx(hDc, SrcRect.X, SrcRect.Y);
                    srcSurface.ReleaseDC(hDc);
                }
                
                device.UpdateSurface(srcSurface, tmpSurface);

                device.StretchRectangle(tmpSurface, destSurface, TextureFilter.Linear);
            }
            catch(SharpDXException ex)
            {
                logger.Error(ex);
                return errorCode;
            }

            var syncRoot = videoBuffer.syncRoot;
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(syncRoot, timeout, ref lockTaken);
                if (lockTaken)
                {
                    //success = BitBlt(srcSurface, videoBuffer.bitmap);

                    bool success = SurfaceToBitmap(destSurface, videoBuffer.bitmap );
                    if (success)
                    {
                        errorCode = ErrorCode.Ok;
                    }
                }

            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(syncRoot);
                }
            }

            //logger.Debug("CopyToBitmap(...) " + sw.ElapsedMilliseconds);

            return errorCode;
        }

        private bool BitBlt(Surface surface, GDI.Bitmap bmp)
        {
            bool success;

            IntPtr hdcDest = IntPtr.Zero;
            GDI.Graphics graphDest = null;

            IntPtr hdcSrc = surface.GetDC();
            try
            {
                graphDest = System.Drawing.Graphics.FromImage(bmp);
                hdcDest = graphDest.GetHdc();
                GDI.Size destSize = bmp.Size;

                int nXDest = 0;
                int nYDest = 0;
                int nWidth = destSize.Width;
                int nHeight = destSize.Height;

                int nXSrc = SrcRect.Left;
                int nYSrc = SrcRect.Top;

                int nWidthSrc = SrcRect.Width;
                int nHeightSrc = SrcRect.Height;

                var dwRop = TernaryRasterOperations.SRCCOPY;

                success = Gdi32.BitBlt(hdcDest, nXDest, nYDest, nWidth, nHeight, hdcSrc, nXSrc, nYSrc, dwRop);
            }
            finally
            {
                graphDest?.ReleaseHdc(hdcDest);
                graphDest?.Dispose();
                graphDest = null;

                surface.ReleaseDC(hdcSrc);

            }

            return success;
        }

        private bool SurfaceToBitmap( Surface surface, GDI.Bitmap bmp)
        {
            bool result = false;

            var surfDescr = surface.Description;

            if (bmp.Width != surfDescr.Width || bmp.Height != surfDescr.Height)
            {
                //...
                logger.Warn("bmp.Width != surfDescr.Width || bmp.Height != surfDescr.Height");
                return result;
            }

            if (!(surfDescr.Format == Format.A8R8G8B8 || surfDescr.Format == Format.X8R8G8B8))
            {
                logger.Warn("Unsupported surface format " + surfDescr.Format);
                return result;
            }

            var bitmapData = bmp.LockBits(new GDI.Rectangle(0, 0, bmp.Width, bmp.Height), GDI.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            try
            {
                DataRectangle dataRect = surface.LockRectangle(LockFlags.ReadOnly);
                try
                {
                    // должен быть одинаковый размер и формат!
                    int height = bmp.Height;
                    int width = bmp.Width;
                    int pictWidth = width * 4;

                    var sourcePtr = dataRect.DataPointer;
                    var destPtr = bitmapData.Scan0;
                    for (int line = 0; line < height; line++)
                    {
                        Utilities.CopyMemory(destPtr, sourcePtr, pictWidth);

                        sourcePtr = IntPtr.Add(sourcePtr, dataRect.Pitch);
                        destPtr = IntPtr.Add(destPtr, bitmapData.Stride);
                    }
                    result = true;                
                }
                finally
                {
                    surface.UnlockRectangle();
                }
            }
            finally
            {
                bmp.UnlockBits(bitmapData);
            }

            return result;

        }


        private bool ReInitDevice()
        {
            logger.Debug("OnReInitDevice");

            bool Result = false;
            try
            {
                OnLostDevice();

                try
                {
                    device.Reset(presentParams);
                    InitSurfaces();
                    Result = true;
                }
                catch (SharpDXException ex)
                {
                    logger.Warn("Graphic device reset result: " + ex.ResultCode);
                }

                /*
                try
                {
                    device?.Dispose();

                    CreateFlags Flags = (CreateFlags.Multithreaded | CreateFlags.FpuPreserve | CreateFlags.HardwareVertexProcessing);
                    // CreateFlags Flags = (CreateFlags.SoftwareVertexProcessing);

                    AdapterInformation adapterInfo = direct3D9.Adapters[0];
                    device = new Device(direct3D9, adapterInfo.Adapter, DeviceType.Hardware, IntPtr.Zero, Flags, presentParams);

                    InitSurfaces();

                    Result = true;

                }
                catch (SharpDXException ex)
                {
                    logger.Warn("Graphic device reset result: " + ex.ResultCode);
                }
                */



            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode == ResultCode.DeviceLost)
                {
                    // deviceLost = true;
                }

                logger.Warn<Exception>(ex);
            }
            catch (Exception ex)
            {
                logger.Error<Exception>(ex);
            }

            return Result;
        }


        private void OnDeviceReset()
        {
            logger.Debug("OnDeviceReset");

            InitSurfaces();

        }

        private void OnLostDevice()
        {
            logger.Debug("OnLostDevice");

           
            DisposeSurfaces();

        }

        private void DisposeSurfaces()
        {
            logger.Debug("DisposeSurfaces()");
            
            if (srcSurface != null && !srcSurface.IsDisposed)
            {

                srcSurface.Dispose();
                srcSurface = null;
            }
            if (destSurface != null && !destSurface.IsDisposed)
            {
                destSurface.Dispose();
                destSurface = null;
            }

            if (tmpSurface != null && !tmpSurface.IsDisposed)
            {
                tmpSurface.Dispose();
                tmpSurface = null;
            }
        }


        public override void Close()
        {
            if (direct3D9 != null && !direct3D9.IsDisposed)
            {
                direct3D9.Dispose();
                direct3D9 = null;
            }

            if (device != null && !device.IsDisposed)
            {
                device.Dispose();
                device = null;
            }

            DisposeSurfaces();

            base.Close();
        }

    }

}
