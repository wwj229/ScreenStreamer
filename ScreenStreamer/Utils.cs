﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenStreamer.Utils
{

    public enum TernaryRasterOperations : uint
    {
        SRCCOPY = 0x00CC0020,
        SRCPAINT = 0x00EE0086,
        SRCAND = 0x008800C6,
        SRCINVERT = 0x00660046,
        SRCERASE = 0x00440328,
        NOTSRCCOPY = 0x00330008,
        NOTSRCERASE = 0x001100A6,
        MERGECOPY = 0x00C000CA,
        MERGEPAINT = 0x00BB0226,
        PATCOPY = 0x00F00021,
        PATPAINT = 0x00FB0A09,
        PATINVERT = 0x005A0049,
        DSTINVERT = 0x00550009,
        BLACKNESS = 0x00000042,
        WHITENESS = 0x00FF0062,
        CAPTUREBLT = 0x40000000 //only if WinVer >= 5.0.0 (see wingdi.h)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }


    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
        public RGBQUAD[] bmiColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }


    //[StructLayout(LayoutKind.Sequential)]
    //public struct RECT
    //{
    //    public int Left;
    //    public int Top;
    //    public int Right;
    //    public int Bottom;
    //}

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public Rectangle AsRectangle
        {
            get
            {
                return new Rectangle(this.Left, this.Top, this.Right - this.Left, this.Bottom - this.Top);
            }
        }

        public static RECT FromXYWH(int x, int y, int width, int height)
        {
            return new RECT(x, y, x + width, y + height);
        }

        public static RECT FromRectangle(Rectangle rect)
        {
            return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }


    [Flags]
    public enum StretchingMode
    {
        /// <summary>
        /// Performs a Boolean AND operation using the color values for the eliminated 
        /// and existing pixels. If the bitmap is a monochrome bitmap, this mode preserves
        /// black pixels at the expense of white pixels.
        /// </summary>
        BLACKONWHITE = 1,
        /// <summary>
        /// Deletes the pixels. This mode deletes all eliminated lines of pixels 
        /// without trying to preserve their information
        /// </summary>
        COLORONCOLOR = 3,
        /// <summary>
        /// Maps pixels from the source rectangle into blocks of pixels in the destination rectangle.
        /// The average color over the destination block of pixels approximates the color of the source pixels. 
        /// This option is not supported on Windows 95/98/Me
        /// </summary>
        HALFTONE = 4,
        /// <summary>
        /// Performs a Boolean AND operation using the color values for the eliminated 
        /// and existing pixels. If the bitmap is a monochrome bitmap, this mode preserves
        /// black pixels at the expense of white pixels (same as BLACKONWHITE)
        /// </summary>
        STRETCH_ANDSCANS = 1,
        /// <summary>
        /// Deletes the pixels. This mode deletes all eliminated lines of pixels 
        /// without trying to preserve their information (same as COLORONCOLOR)
        /// </summary>
        STRETCH_DELETESCANS = 3,
        /// <summary>
        /// Maps pixels from the source rectangle into blocks of pixels in the destination rectangle.
        /// The average color over the destination block of pixels approximates the color of the source pixels. 
        /// This option is not supported on Windows 95/98/Me (same as HALFTONE)
        /// </summary>
        STRETCH_HALFTONE = 4,
        /// <summary>
        /// Performs a Boolean OR operation using the color values for the eliminated and existing pixels.
        /// If the bitmap is a monochrome bitmap, this mode preserves white pixels at the expense of 
        /// black pixels(same as WHITEONBLACK)
        /// </summary>
        STRETCH_ORSCANS = 2,
        /// <summary>
        /// Performs a Boolean OR operation using the color values for the eliminated and existing pixels.
        /// If the bitmap is a monochrome bitmap, this mode preserves white pixels at the expense of black pixels.
        /// </summary>
        WHITEONBLACK = 2,
        /// <summary>
        /// Fail to stretch
        /// </summary>
        ERROR = 0
    }

    public sealed class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest,
                                        IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
                                        TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        public static extern int SetStretchBltMode(IntPtr hdc, StretchingMode iStretchMode);


        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
    }

    public sealed class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CopyMemory")]
        public static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

        [DllImport("kernel32.dll", CharSet = CharSet.None, EntryPoint = "RtlZeroMemory", ExactSpelling = false)]
        public static extern void ZeroMemory(IntPtr ptr, int size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTimes(out System.Runtime.InteropServices.ComTypes.FILETIME lpIdleTime,
            out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime,
            out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime);
    }


    [System.Security.SuppressUnmanagedCodeSecurity()]
    public sealed class User32
    {

        internal const uint MONITOR_MONITOR_DEFAULTTONULL = 0x00000000;
        internal const uint MONITOR_MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        internal const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        internal static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

        public static IntPtr GetMonitorFromRect(Rectangle screen)
        {
            RECT rect = new RECT
            {
                Left = screen.Left,
                Top = screen.Top,
                Right = screen.Right,
                Bottom =screen.Bottom,
            };

            return MonitorFromRect(ref rect, MONITOR_MONITOR_DEFAULTTOPRIMARY);
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);



        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);


        public static Rectangle GetClientRect(IntPtr hwnd)
        {
            RECT rect = new RECT();
            GetClientRect(hwnd, out rect);
            return rect.AsRectangle;
        }

        public static Rectangle GetWindowRect(IntPtr hwnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hwnd, out rect);
            return rect.AsRectangle;
        }

        public static Rectangle GetAbsoluteClientRect(IntPtr hWnd)
        {
            Rectangle windowRect = User32.GetWindowRect(hWnd);
            Rectangle clientRect = User32.GetClientRect(hWnd);

            int chromeWidth = (int)((windowRect.Width - clientRect.Width) / 2);

            return new Rectangle(new Point(windowRect.X + chromeWidth, windowRect.Y + (windowRect.Height - clientRect.Height - chromeWidth)), clientRect.Size);
        }

    }

    [Flags]
    public enum CompositionAction : uint
    {
        DWM_EC_DISABLECOMPOSITION = 0,
        DWM_EC_ENABLECOMPOSITION = 1
    }

    class DwmApi
    {
        public static void DisableAero(bool disable)
        {
            var compositionAction = disable ? CompositionAction.DWM_EC_DISABLECOMPOSITION : CompositionAction.DWM_EC_ENABLECOMPOSITION;

            DwmEnableComposition(compositionAction);

        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern void DwmEnableComposition(CompositionAction uCompositionAction);

    }

    public class RngProvider
    {
        private static System.Security.Cryptography.RNGCryptoServiceProvider provider =
            new System.Security.Cryptography.RNGCryptoServiceProvider();

        public static uint GetRandomNumber()
        {
            byte[] bytes = new byte[sizeof(UInt32)];
            provider.GetNonZeroBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }


    public class MediaTimer
    {
        public const long TicksPerMillisecond = 10000;
        public const long TicksPerSecond = TicksPerMillisecond * 1000;

        public static double GetRelativeTimeMilliseconds()
        {
            return (Ticks / (double)TicksPerMillisecond);
        }

        public static double GetRelativeTime()
        {
            return (Ticks / (double)TicksPerSecond);
        }

        public static long Ticks
        {
            get
            {
                return (long)(Stopwatch.GetTimestamp() * TicksPerSecond / (double)Stopwatch.Frequency);
                //return DateTime.Now.Ticks;
                //return NativeMethods.timeGetTime() * TicksPerMillisecond;
            }
        }

        public static DateTime GetDateTimeFromNtpTimestamp(ulong ntmTimestamp)
        {
            uint TimestampMSW = (uint)(ntmTimestamp >> 32);
            uint TimestampLSW = (uint)(ntmTimestamp & 0x00000000ffffffff);

            return GetDateTimeFromNtpTimestamp(TimestampMSW, TimestampLSW);
        }

        public static DateTime GetDateTimeFromNtpTimestamp(uint TimestampMSW, uint TimestampLSW)
        {
            /*
            Timestamp, MSW: 3670566484 (0xdac86654)
            Timestamp, LSW: 3876982392 (0xe7160e78)
            [MSW and LSW as NTP timestamp: Apr 25, 2016 09:48:04.902680000 UTC]
             * */

            DateTime ntpDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            uint ntpTimeMilliseconds = (uint)(Math.Round((double)TimestampLSW / (double)uint.MaxValue, 3) * 1000);
            return ntpDateTime
                .AddSeconds(TimestampMSW)
                .AddMilliseconds(ntpTimeMilliseconds);
        }

        private DateTime startDateTime;
        private long startTimestamp;

        private bool isRunning = false;

        public void Start(DateTime dateTime)
        {
            if (isRunning == false)
            {
                startDateTime = dateTime;
                startTimestamp = Stopwatch.GetTimestamp();

                isRunning = true;
            }
        }

        public DateTime Now
        {
            get
            {
                DateTime dateTime = DateTime.MinValue;
                if (isRunning)
                {
                    dateTime = startDateTime.AddTicks(ElapsedTicks);
                }

                return dateTime;
            }
        }

        public TimeSpan Elapsed
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                if (isRunning)
                {
                    timeSpan = new TimeSpan(ElapsedTicks);
                }
                return timeSpan;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                long ticks = 0;
                if (isRunning)
                {
                    ticks = (long)((Stopwatch.GetTimestamp() - startTimestamp) * TicksPerSecond / (double)Stopwatch.Frequency);

                    if (ticks < 0)
                    {
                        Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! ticks " + ticks);
                    }
                }
                return ticks;
            }
        }

        public void Stop()
        {

            if (isRunning)
            {
                isRunning = false;
            }

        }
    }


    public class PerfCounter : IDisposable
    {
        public PerfCounter()
        {
            _PerfCounter();
        }

        private void _PerfCounter()
        {
            closed = false;
            isFirstUpdate = true;
            isFirstPresent = true;

            thread = new Thread(TimerTick);
            thread.Start();
        }

        private Thread thread = null;

        private short CPU = 0;

        private double meanUpdatePerSec = 0;
        private double meanPresentPerSec = 0;

        private long lastUpdate = 0;
        private long lastPresent = 0;

        private bool isFirstUpdate = true;
        private bool isFirstPresent = true;

        public void UpdateSignalStats()
        {
            long timestamp = Stopwatch.GetTimestamp();
            if (!isFirstUpdate)
            {
                double elapsed = (timestamp - lastUpdate) / (double)Stopwatch.Frequency;

                if (elapsed > 0)
                {
                    meanUpdatePerSec = (1.0 / elapsed) * 0.05 + (1 - 0.05) * meanUpdatePerSec;
                }
                else
                {
                    Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!! elapsed " + elapsed);
                }
            }
            else
            {
                isFirstUpdate = false;
            }
            lastUpdate = timestamp;
        }

        public void UpdatePresentStats()
        {
            long timestamp = Stopwatch.GetTimestamp();

            if (!isFirstPresent)
            {
                double elapsed = (timestamp - lastPresent) / (double)Stopwatch.Frequency;

                if (elapsed > 0)
                {
                    meanPresentPerSec = (1.0 / elapsed) * 0.05 + (1 - 0.05) * meanPresentPerSec;
                }
                else
                {
                    Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!! elapsed " + elapsed);
                }
            }
            else
            {
                isFirstPresent = false;
            }

            lastPresent = timestamp;
        }
        
        public string GetCpuUsage()
        {
            string cpuUsage = "";
            if(CPU >= 0 && CPU <= 100)
            {
                cpuUsage = "CPU=" + CPU + "%";
            }
            else
            {
                cpuUsage = "CPU=--%";
            }
            return cpuUsage;
        }

        public string GetReport2()
        {

            return string.Format("FPS={0:0.0} FPS2={1:0.0} CPU={2}%", meanUpdatePerSec, meanPresentPerSec, CPU);
        }

        private void TimerTick(object state)
        {
            Debug.WriteLine("TimerTick() BEGIN");

            if (closed == false)
            {
                CPUCounter counter = null;
                try
                {
                    counter = new CPUCounter();
                    while (true)
                    {
                        if (closed)
                        {
                            break;
                        }

                        CPU = counter.GetUsage();

                        syncEvent.WaitOne(1000);
                    }

                }
                finally
                {

                    if (counter != null)
                    {
                        counter.Dispose();
                        counter = null;
                    }
                }
            }

            Debug.WriteLine("TimerTick() END");
        }

        private AutoResetEvent syncEvent = new AutoResetEvent(true);
        private volatile bool closed = false;
        public void Dispose()
        {
            closed = true;
            syncEvent.Set();
        }

        class CPUCounter : IDisposable
        {

            private System.Runtime.InteropServices.ComTypes.FILETIME prevSysKernel;
            private System.Runtime.InteropServices.ComTypes.FILETIME prevSysUser;

            private TimeSpan prevProcTotal;

            private short CPUUsage;
            //DateTime LastRun;

            private long lastTimestamp;

            private long runCount;

            private Process currentProcess;

            public CPUCounter()
            {
                CPUUsage = -1;
                lastTimestamp = 0;

                //LastRun = DateTime.MinValue;
                prevSysUser.dwHighDateTime = prevSysUser.dwLowDateTime = 0;
                prevSysKernel.dwHighDateTime = prevSysKernel.dwLowDateTime = 0;
                prevProcTotal = TimeSpan.MinValue;
                runCount = 0;

                currentProcess = Process.GetCurrentProcess();
            }

            public short GetUsage()
            {
                if (disposed)
                {
                    return 0;
                }

                short CPUCopy = CPUUsage;
                if (Interlocked.Increment(ref runCount) == 1)
                {
                    if (!EnoughTimePassed)
                    {
                        Interlocked.Decrement(ref runCount);
                        return CPUCopy;
                    }

                    System.Runtime.InteropServices.ComTypes.FILETIME sysIdle, sysKernel, sysUser;
                    if (!Kernel32.GetSystemTimes(out sysIdle, out sysKernel, out sysUser))
                    {
                        Interlocked.Decrement(ref runCount);
                        return CPUCopy;
                    }

                    //Process process = Process.GetCurrentProcess();
                    TimeSpan procTime = currentProcess.TotalProcessorTime;

                    if (!isFirstRun)
                    {
                        UInt64 sysKernelDiff = SubtractTimes(sysKernel, prevSysKernel);
                        UInt64 sysUserDiff = SubtractTimes(sysUser, prevSysUser);
                        UInt64 sysTotal = sysKernelDiff + sysUserDiff;

                        Int64 procTotal = procTime.Ticks - prevProcTotal.Ticks;
                        // long procTotal = (long)((Stopwatch.GetTimestamp() - lastTimestamp) * 10000000.0 / (double)Stopwatch.Frequency);
                        if (sysTotal > 0)
                        {
                            CPUUsage = (short)((100.0 * procTotal) / sysTotal);
                        }
                    }
                    else
                    {
                        isFirstRun = false;
                    }

                    prevProcTotal = procTime;
                    prevSysKernel = sysKernel;
                    prevSysUser = sysUser;

                    //LastRun = DateTime.Now;

                    lastTimestamp = Stopwatch.GetTimestamp();

                    CPUCopy = CPUUsage;
                }
                Interlocked.Decrement(ref runCount);

                return CPUCopy;

            }

            private UInt64 SubtractTimes(System.Runtime.InteropServices.ComTypes.FILETIME a, System.Runtime.InteropServices.ComTypes.FILETIME b)
            {
                UInt64 aInt = ((UInt64)(a.dwHighDateTime << 32)) | (UInt64)a.dwLowDateTime;
                UInt64 bInt = ((UInt64)(b.dwHighDateTime << 32)) | (UInt64)b.dwLowDateTime;

                return aInt - bInt;
            }

            private bool EnoughTimePassed
            {
                get
                {
                    const int minimumElapsedMS = 250;

                    long ticks = (long)((Stopwatch.GetTimestamp() - lastTimestamp) * 10000000.0 / (double)Stopwatch.Frequency);
                    TimeSpan sinceLast = new TimeSpan(ticks);

                    //TimeSpan sinceLast = DateTime.Now - LastRun;


                    return sinceLast.TotalMilliseconds > minimumElapsedMS;
                }
            }

            private bool isFirstRun = true;
            //{
            //    get
            //    {
            //        return (lastTimestamp == 0);
            //        //return (LastRun == DateTime.MinValue);
            //    }
            //}

            private volatile bool disposed = false;
            public void Dispose()
            {
                disposed = true;

                if (currentProcess != null)
                {
                    currentProcess.Dispose();
                    currentProcess = null;
                }
            }
        }

    }
}
