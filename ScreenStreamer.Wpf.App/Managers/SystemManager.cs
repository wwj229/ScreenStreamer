﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaToolkit.Utils;
using MediaToolkit.NativeAPIs;

using Microsoft.Win32;
using System.Windows;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Diagnostics;
using MediaToolkit.Managers;
using NLog;
using ScreenStreamer.Wpf.Models;

namespace ScreenStreamer.Wpf.Managers
{
	public class SystemManager : IWndMessageProcessor
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private Dictionary<Guid, IntPtr> notifyHandles = null;

		private NotifyWindow notifyWindow = null;

		private List<VideoSourceItem> videoSources = null;
		private List<AudioSourceItem> audioSources = null;
		private List<EncoderItem> h264Encoders = null;
		private List<ScreenCaptureItem> screenCaptures = null;

		public event Action<object> VideoSourcesChanged;
		public event Action<object> AudioSourcesChanged;

		public bool Initialized { get; private set; } = false;

		public bool Initialize()
		{
			logger.Debug("SystemManager::Initialize() " + Initialized);
			if (!Initialized)
			{
				try
				{
					SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
					SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

					notifyWindow = new NotifyWindow(this);

					notifyWindow.CreateWindow();

					var hWnd = notifyWindow.Handle;
					Debug.Assert(hWnd != IntPtr.Zero, "hWnd != IntPtr.Zero");

					RegisterNotification(hWnd, UsbCategory.VideoCamera);

					RegisterNotification(hWnd, UsbCategory.Audio);

					Initialized = true;

				}
				catch (Exception ex)
				{
					logger.Error(ex.Message);
					Shutdown();
				}
			}

			return Initialized;
		}

		private void RegisterNotification(IntPtr hWnd, Guid classId)
		{
			if(notifyHandles == null)
			{
				notifyHandles = new Dictionary<Guid, IntPtr>();
			}

			var notifyHandle = UsbDeviceManager.RegisterNotification(hWnd, classId);
			if (notifyHandle != IntPtr.Zero)
			{
				notifyHandles.Add(classId, notifyHandle);
			}
			else
			{
				logger.Warn($"Fail to register {classId} notifications");
			}
		}

		bool IWndMessageProcessor.ProcessMessage(Message m)
		{
			bool result = false;

			switch ((uint)m.Msg)
			{
				case WM.DEVICECHANGE:
					{
						uint eventCode = (uint)m.WParam;

						if (eventCode == DBT.DEVICEARRIVAL || eventCode == DBT.DEVICEREMOVECOMPLETE)
						{
							if (UsbDeviceManager.TryPtrToDeviceInfo(m.LParam, out UsbDeviceInfo di))
							{   // получили информацию о подключенном устройстве в виде:
								// \\?\USB#VID_0A89&PID_000C#6&2c24ce2e&0&4#{a5dcbf10-6530-11d2-901f-00c04fb951ed}

								bool deviceArraval = (eventCode == DBT.DEVICEARRIVAL);

								if (di.ClassGuid == UsbCategory.Audio || di.ClassGuid == UsbCategory.AudioDevice)
								{ // update audio sources...
									logger.Debug("Audio USB " + (deviceArraval ? "arrival: " : "moved: ") + di.FriendlyName + " {" + di.Name + "}");
									VideoSourcesChanged?.Invoke(di.Name);
								}
								else if (di.ClassGuid == UsbCategory.VideoCamera || di.ClassGuid == UsbCategory.Video)
								{// update video sources...
									logger.Debug("Video USB " + (deviceArraval ? "arrival: " : "moved: ") + di.FriendlyName + " {" + di.Name + "}");

									AudioSourcesChanged?.Invoke(di.Name);
								}
								else
								{

								}

								result = true;
							}
							else
							{//TODO:
							 //...

							}
						}

						//logger.Debug("WM_DEVICECHANGE");
						break;
					}

			}

			return result;
		}

		private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			logger.Debug("SystemEvents_DisplaySettingsChanged(...)");
			//...
		}

		private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			logger.Debug("SystemEvents_SessionSwitch(...) " + e.Reason);

			//...
		}


		public void Shutdown()
		{
			logger.Debug("SystemManager::Shutdown() " + Initialized);

			//if (!Initialized)
			//{
			//	return;
			//}

			SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
			SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;

			if (notifyHandles != null && notifyHandles.Count > 0)
			{
				foreach (var handle in notifyHandles.Values)
				{
					var result = UsbDeviceManager.UnregisterNotification(handle);
				}
			}

			if (notifyWindow != null)
			{
				notifyWindow.DestroyWindow();

				notifyWindow = null;
			}

			Initialized = false;
		}
	}


}
