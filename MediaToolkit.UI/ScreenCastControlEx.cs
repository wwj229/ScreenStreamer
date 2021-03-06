﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MediaToolkit.UI;

using System.Windows.Threading;

using System.ServiceModel;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Globalization;
using MediaToolkit;
using MediaToolkit.Core;
using System.ServiceModel.Discovery;
using MediaToolkit.Utils;

using MediaToolkit.SharedTypes;
using System.Diagnostics;
using MediaToolkit.Logging;

namespace MediaToolkit.UI
{
    public partial class ScreenCastControlEx : UserControl, IScreenCasterControl
    {
        private static readonly TraceSource logger = TraceManager.GetTrace("MediaToolkit.UI");

        //private static Logger logger = LogManager.GetCurrentClassLogger();
         
        public ScreenCastControlEx()
        {
            InitializeComponent();

            syncContext = SynchronizationContext.Current;

            debugPanel.Visible = false;


            _UpdateControls();


        }


        private volatile ClientState state = ClientState.Disconnected;
        public ClientState State => state;

        public event Action Connected;
        public event Action<object> Disconnected;

        private volatile ErrorCode errorCode = ErrorCode.Ok;
        public ErrorCode Code => errorCode;

        private readonly SynchronizationContext syncContext = null;
        private AutoResetEvent syncEvent = new AutoResetEvent(false);
        private volatile bool cancelled = false;

        public VideoReceiverEx VideoReceiver { get; private set; }
        public AudioReceiver AudioReceiver { get; private set; }

        public D3D9RendererSink videoRendererSink { get; private set; }

        public string ClientId { get; private set; }
        public string ServerId { get; private set; }
        public string ServerName { get; private set; }
        public string ServerAddr { get; private set; }
        public int ServerPort { get; private set; }

        private ChannelFactory<IScreenCastService> factory = null;

        private IntPtr VideoHandle = IntPtr.Zero;

        public uint MaxTryConnectCount { get; set; } = uint.MaxValue; //5

        private bool aspectRatio = true;
        public bool AspectRatio
        {
            get
            { 
                return aspectRatio;
            }
            set
            {
                aspectRatio = value;
            }
        }


        public bool ShowDebugPanel
        {
            get
            {
                return debugPanel.Visible;
            }
            set
            {
                if (debugPanel.Visible != value)
                {
                    debugPanel.Visible = value;
                }
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            logger.Verb("ScreenCastControl::connectButton_Click(...)");

           // logger.Debug("connectButton_Click()");

            try
            {
                if (state == ClientState.Disconnected)
                {
                    var addrStr = hostAddressTextBox.Text;

                    var uri = new Uri("net.tcp://" + addrStr);

                    logger.Info("Connect to: " + uri.ToString());
                    //logger.Info("Connect to: " + uri.ToString());

                    var host = uri.Host;
                    var port = uri.Port;

                    Connect(host, port);
                }
                else
                {
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                logger.Error(ex);
            }

        }

        private void Factory_Closed(object sender, EventArgs e)
        {
            //logger.Debug("Factory_Closed()");
            logger.Verb("ScreenCastControl::Factory_Closed()");
        }


        private void videoRenderer_RenderStarted()
        {
            logger.Verb("ScreenCastControl::videoRenderer_RenderStarted()");
            //logger.Debug("videoRenderer_RenderStarted()");
        }

        private void videoRenderer_RenderStopped(object obj)
        {
            logger.Verb("ScreenCastControl::videoRenderer_RenderStopped()");

            //logger.Debug("videoRenderer_RenderStopped(...) ");


       
        }

        
        protected override void OnResize(EventArgs e)
        {
            videoRendererSink?.Resize(this.ClientRectangle);
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            videoRendererSink?.Repaint();
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }


        private Task mainTask = null;
        public void Connect(string addr, int port)
        {
            logger.Verb("ScreenCastControl::Connecting(...) " + addr + " " + port);
            VideoHandle = this.Handle;
            //logger.Debug("RemoteDesktopClient::Connecting(...) " + addr + " " + port);

            state = ClientState.Connecting;

            cancelled = false;
            errorCode = ErrorCode.Ok;

            this.ServerAddr = addr;
            this.ServerPort = port;

            this.ClientId = RngProvider.GetRandomNumber().ToString();

            var address = "net.tcp://" + ServerAddr + "/ScreenCaster";
            if (this.ServerPort > 0)
            {
                address = "net.tcp://" + ServerAddr + ":" + ServerPort + "/ScreenCaster";
            }

            //Console.WriteLine(address);

            UpdateControls();
            var clientRect = this.ClientRectangle;
            mainTask = Task.Run(() =>
            {
                uint tryCount = 0;

                bool forceReconnect = (MaxTryConnectCount == uint.MaxValue);

                while ((forceReconnect || tryCount <= MaxTryConnectCount) && !cancelled)
                {
                    logger.Verb("ScreenCastControl::Connecting count: " + tryCount);

                    //logger.Debug("Connecting count: " + tryCount);
                    //errorMessage = "";
                    errorCode = ErrorCode.Ok;

                    try
                    {
                        var uri = new Uri(address);

                        NetTcpSecurity security = new NetTcpSecurity
                        {
                            Mode = SecurityMode.None,
                        };

                        var binding = new NetTcpBinding
                        {
                            ReceiveTimeout = TimeSpan.MaxValue,//TimeSpan.FromSeconds(10),
                            SendTimeout = TimeSpan.FromSeconds(5),
                            OpenTimeout = TimeSpan.FromSeconds(5),

                            Security = security,
                        };

                        factory = new ChannelFactory<IScreenCastService>(binding, new EndpointAddress(uri));
                        factory.Closed += Factory_Closed;

                        var channel = factory.CreateChannel();

                        try
                        {
                            var channelInfos = channel.GetChannelInfos();

                            state = ClientState.Connected;
                            UpdateControls();
                            Connected?.Invoke();

                            if (channelInfos == null)
                            {
                                errorCode = ErrorCode.NotReady;
                                throw new Exception("Server not configured");
                            }

                            var videoChannelInfo = channelInfos.FirstOrDefault(c => c.MediaInfo is VideoChannelInfo);
                            if (videoChannelInfo != null)
                            {
                                if (videoChannelInfo.Transport == TransportMode.Tcp && videoChannelInfo.ClientsCount > 0)
                                {
                                    errorCode = ErrorCode.IsBusy;
                                    throw new Exception("Server is busy");
                                }
                                SetupVideo(videoChannelInfo);

                                videoRendererSink.Resize(clientRect);
                            }

                            var audioChannelInfo = channelInfos.FirstOrDefault(c => c.MediaInfo is AudioChannelInfo);
                            if (audioChannelInfo != null)
                            {
                                if (audioChannelInfo.Transport == TransportMode.Tcp && videoChannelInfo.ClientsCount > 0)
                                {
                                    errorCode = ErrorCode.IsBusy;
                                    throw new Exception("Server is busy");
                                }

                                SetupAudio(audioChannelInfo);
                            }

                            if (VideoReceiver != null)
                            {
                                VideoReceiver.Play();

                                videoRendererSink.Start();
                            }

                            if (AudioReceiver != null)
                            {
                                AudioReceiver.Play();
                            }

                            channel.PostMessage(new ServerRequest { Command = "Ping" });
                            tryCount = 0;                           
   
                            state = ClientState.Running;
                            UpdateControls();

                            while (state == ClientState.Running)
                            {
                                try
                                {
                                    channel.PostMessage(new ServerRequest { Command = "Ping" });
                                    if (videoRendererSink.ErrorCode != 0)
                                    {
                                        logger.Warn("ScreenCastControl::imageProvider.ErrorCode: " + videoRendererSink.ErrorCode);

                                       // logger.Debug("imageProvider.ErrorCode: " + videoRenderer.ErrorCode);
                                        //Process render error...
                                    }

                                    //TODO::
                                    // Receivers errors...

                                    syncEvent.WaitOne(1000);
                                }
                                catch (Exception ex)
                                {
                                    state = ClientState.Interrupted;
                                    errorCode = ErrorCode.Interrupted;
                                }
                            }
                        }
                        finally
                        {
                            CloseChannel(channel);
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        errorCode = ErrorCode.NotFound;

                        logger.Error(ex.Message);

                        //logger.Error(ex.Message);

                        //Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        //logger.Error(ex);

                        if (errorCode == ErrorCode.Ok)
                        {
                            errorCode = ErrorCode.Fail;
                        }

                        //Console.WriteLine(ex);
                    }
                    finally
                    {
                        Close();
                    }

                    if (!cancelled)
                    {

                        if (errorCode != ErrorCode.Ok)
                        {
                            UpdateControls();

                            tryCount++;

                            //var statusStr = "Attempting to connect: " + tryCount + " of " + maxTryCount;
                            var statusStr = "Attempting to connect: " + tryCount;

                            SetStatus(statusStr);
                        }

                    }
                    else
                    {
                        errorCode = ErrorCode.Cancelled;
                    }
                }

                cancelled = false;

                state = ClientState.Disconnected;
                UpdateControls();

                Disconnected?.Invoke(null);

                Console.WriteLine(SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects());
            });
        }


        public void Disconnect()
        {
            state = ClientState.Disconnecting;
            cancelled = true;
            //factory.Abort();
            UpdateControls();

        }


        private void SetupAudio(ScreencastChannelInfo audioChannelInfo)
        {
            logger.Verb("ScreenCastControl::SetupAudio(...)");

            //logger.Debug("SetupAudio(...)");

            var audioInfo = audioChannelInfo.MediaInfo as AudioChannelInfo;
            if (audioInfo == null)
            {
                return;
            }

            var audioAddr = audioChannelInfo.Address;
            if (audioChannelInfo.Transport == TransportMode.Tcp)
            {
                audioAddr = ServerAddr;
            }

            var audioPort = audioChannelInfo.Port;
            AudioReceiver = new AudioReceiver();
            var networkPars = new NetworkSettings
            {
                LocalAddr = audioAddr,
                LocalPort = audioPort,
                TransportMode = audioChannelInfo.Transport,
                SSRC = audioChannelInfo.SSRC,

            };

            var audioDeviceId = "";
            try
            {
                var devices = NAudio.Wave.DirectSoundOut.Devices;
                var device = devices.FirstOrDefault();
                audioDeviceId = device?.Guid.ToString() ?? "";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //logger.Error(ex);
            }

            var audioPars = new AudioEncoderSettings
            {
                SampleRate = audioInfo.SampleRate,
                Channels = audioInfo.Channels,
                Encoding = "ulaw",
                DeviceId = audioDeviceId,
            };

            AudioReceiver.Setup(audioPars, networkPars);

        }


        private void SetupVideo(ScreencastChannelInfo videoChannelInfo)
        {
            logger.Verb("ScreenCastControl::SetupVideo(...)");


           //logger.Debug("SetupVideo(...)");

            var videoInfo = videoChannelInfo.MediaInfo as VideoChannelInfo;
            if (videoInfo == null)
            {
                return;
            }


            var videoAddr = videoChannelInfo.Address;

            if (videoChannelInfo.Transport == TransportMode.Tcp)
            {
                videoAddr = ServerAddr;
            }

            var videoPort = videoChannelInfo.Port;
            var encoderSettings = new VideoEncoderSettings
            {
                //Resolution = videoInfo.Resolution,
                Width = videoInfo.Resolution.Width,
                Height = videoInfo.Resolution.Height,
                FrameRate = new MediaRatio(videoInfo.Fps, 1),
            };

            var networkSettings = new NetworkSettings
            {
                LocalAddr = videoAddr,
                LocalPort = videoPort,
                TransportMode = videoChannelInfo.Transport,
                SSRC = videoChannelInfo.SSRC,
            };

            VideoReceiver = new VideoReceiverEx();

            VideoReceiver.DataReceived += VideoReceiver_DataReceived;

            VideoReceiver.Setup(networkSettings);

            videoRendererSink = new D3D9RendererSink();
            encoderSettings.LowLatency = true;
            encoderSettings.UseHardware = true;
           

            videoRendererSink.Setup(encoderSettings, VideoHandle);

        }



        double prevTime = 0;
        private void VideoReceiver_DataReceived(byte[] data, double time)
        {

            //logger.Debug("time " + time + " " + (time - prevTime));

            videoRendererSink?.ProcessData(data, time);
            // videoRenderer?.ProcessData(data, 0);

            //prevTime += 0.033;
            //prevTime = time;
        }


        public void Close()
        {
            if (videoRendererSink != null)
            {
                videoRendererSink.Stop();
               //videoRendererSink.Close();
                videoRendererSink = null;

            }

            if (VideoReceiver != null)
            {
                VideoReceiver.DataReceived -= VideoReceiver_DataReceived;
                VideoReceiver.Stop();
                VideoReceiver = null;
            }

            if (AudioReceiver != null)
            {
                AudioReceiver.Stop();
                AudioReceiver = null;
            }

            if (factory != null)
            {
                factory.Closed -= Factory_Closed;

                factory.Abort();
                factory = null;
            }

            //state = ClientState.Disconnected;

        }



        private DiscoveryClient discoveryClient = null;
        private bool finding = false;

        private void findServiceButton_Click(object sender, EventArgs e)
        {
            logger.Verb("ScreenCastControl::findServiceButton_Click(...)");

            //logger.Debug("findServiceButton_Click(...)");

            if (!finding)
            {
                var udpDiscoveryEndpoint = new UdpDiscoveryEndpoint();
                udpDiscoveryEndpoint.EndpointBehaviors.Add(new WcfDiscoveryAddressCustomEndpointBehavior());

                if (discoveryClient == null)
                {
                    discoveryClient = new DiscoveryClient(udpDiscoveryEndpoint);

                    discoveryClient.FindCompleted += DiscoveryClient_FindCompleted;
                    discoveryClient.FindProgressChanged += DiscoveryClient_FindProgressChanged;
                }

                var criteria = new FindCriteria(typeof(IScreenCastService));
                criteria.Duration = TimeSpan.FromSeconds(5);

                finding = true;
                findServiceButton.Text = "_Cancel";
                labelStatus.Text = "_Finding...";

                connectButton.Enabled = false;
                hostsComboBox.Enabled = false;
                hostsComboBox.DataSource = null;

                discoveryClient.FindAsync(criteria, this);
            }
            else
            {
                if (discoveryClient != null)
                {
                    discoveryClient.CancelAsync(this);
                    discoveryClient.Close();
                }
            }
        }



        private void DiscoveryClient_FindProgressChanged(object sender, FindProgressChangedEventArgs e)
        {
            logger.Verb("ScreenCastControl::FindProgressChanged(...) " + e.EndpointDiscoveryMetadata.Address.ToString());


            //logger.Debug("FindProgressChanged(...) " + e.EndpointDiscoveryMetadata.Address.ToString());
        }

        private void DiscoveryClient_FindCompleted(object sender, FindCompletedEventArgs e)
        {
            logger.TraceEvent(TraceEventType.Verbose, 0, "ScreenCastControl::FindCompleted(...)");

           // logger.Debug("FindCompleted(...)");

            finding = false;

            List<ComboBoxItem> hostItems = new List<ComboBoxItem>();

            if (e.Cancelled)
            {
                //logger.Debug("Cancelled");
                logger.Verb("ScreenCastControl::FindCompleted(...) Cancelled");
            }
            if (e.Error != null)
            {
               // logger.Debug(e.Error.ToString());
                logger.Error(e.Error);

            }


            if (!e.Cancelled)
            {
                var result = e.Result;
                if (result != null)
                {


                    foreach (var ep in result.Endpoints)
                    {
                        string address = ep.Address.ToString();
                        string hostName = address;

                        var extensions = ep.Extensions;
                        if (extensions != null && extensions.Count > 0)
                        {
                            var hostElement = extensions.FirstOrDefault(el => el.Name == "HostName");
                            if (hostElement != null)
                            {
                                hostName = hostElement.Value;// + " {" + address + "}";
                            }
                        }

                        //logger.Debug(hostName);

                        hostItems.Add(new ComboBoxItem
                        {
                            Name = hostName,
                            Tag = address,
                        });
                    }
                }

            }

            hostsComboBox.DataSource = hostItems;
            hostsComboBox.DisplayMember = "Name";

            discoveryClient.Close();
            discoveryClient = null;

            connectButton.Enabled = true;
            hostsComboBox.Enabled = true;

            findServiceButton.Text = "_Find";
            labelStatus.Text = "_Not Connected";
        }


        private void hostsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var obj = hostsComboBox.SelectedItem;

            if (obj != null)
            {
                var item = obj as ComboBoxItem;
                if (item != null)
                {
                    var tag = item.Tag;
                    if (tag != null)
                    {
                        var addr = tag.ToString();
                        try
                        {
                            var builder = new UriBuilder(addr);

                            hostAddressTextBox.Text = builder.Host + ":" + builder.Port;
                        }
                        catch (Exception ex)
                        {
                            logger.TraceData(TraceEventType.Verbose, 0, ex);
                            //logger.Debug(ex);
                        }

                    }

                }
            }
        }

        private void UpdateControls()
        {
            //logger.Debug("UpdateControls(...) " + state);

            syncContext.Send(_ =>
            {
                _UpdateControls();

            }, null);

        }

        private void SetStatus(string text)
        {
            syncContext.Send(_ =>
            {
                labelStatus.Text = text;
                statusLabel.Text = "Connecting...";
                //imageProvider.Status = "Connecting...";
                //statusLabel.Text = text;

            }, null);
        }

        private void _UpdateControls()
        {

            bool isDisconnected = (state == ClientState.Disconnected);
            bool isConnected = (state == ClientState.Connected);

            //connectButton.Enabled = !isConnected;
            if (isDisconnected)
            {
                connectButton.Text = "_Connect";

                //wpfRemoteControl.DataContext = null;

                string _statusStr = "";//"Waiting for connection";
                string labelStatusStr = "_Not Connected";

                if (errorCode!= ErrorCode.Ok)
                {
                    //_statusStr = errorCode.ToString();

                    _statusStr = "Connection error";
                    if (errorCode == ErrorCode.Interrupted)
                    {
                        _statusStr = "The connection has been lost";
                    }
                    else if (errorCode == ErrorCode.NotFound)
                    {
                        _statusStr = "Server not found";
                    }
                    else if (errorCode == ErrorCode.NotReady)
                    {
                        _statusStr = "Server not configured";
                    }

                    labelStatusStr = _statusStr;

                    if (errorCode == ErrorCode.Cancelled)
                    {
                        labelStatusStr = "_Not Connected";
                        _statusStr = "";
                    }

                    //Server Disconnected
                    //_Connection Error
                }

                statusLabel.Text = _statusStr;
                //imageProvider.Status = "_statusStr";

                labelStatus.Text = labelStatusStr;
            }
            else
            {
                if(state == ClientState.Running)
                {
                    connectButton.Text = "_Disconnect";
                    labelStatus.Text = "_Connected";

                    //imageProvider.Status = "";
                    statusLabel.Text = "";
                    //wpfRemoteControl.DataContext = imageProvider;
                }
                else if(state == ClientState.Connecting)
                {
                    //wpfRemoteControl.DataContext = imageProvider;

                    labelStatus.Text = "_Connecting...";

                    statusLabel.Text = "Connecting...";

                   // imageProvider.Status = "Connecting...";
                    //imageProvider.Status = "_Connecting...";

                    //controlPanel.Enabled = false;
                    this.hostAddressTextBox.Text = ServerAddr + ":" + ServerPort;

                    connectButton.Text = "_Cancel";
                }
                else if (state == ClientState.Disconnecting)
                {
                    //labelStatus.Text = "_Disconnecting...";
                }
                else
                {
                    //connectButton.Text = "_Cancel";
                }

            }

            if (cancelled)
            {
                labelStatus.Text = "_Cancelling...";

                statusLabel.Text = "Cancelling...";

                //videoRenderer.Status = "Cancelling...";
            }

            //controlPanel.Enabled = true;

            findServiceButton.Enabled = isDisconnected;
            hostsComboBox.Enabled = isDisconnected;
            hostAddressTextBox.Enabled = isDisconnected;


            showDetailsButton.Text = controlPanel.Visible ? "<<" : ">>";

           
            //labelInfo.Text = errorMessage;
        }

        private void detailsButton_Click(object sender, EventArgs e)
        {
            //controlPanel.Visible = !controlPanel.Visible;

            //detailsButton.Text = mainPanel.Visible ? "<<" : ">>";
        }

        private void showDetailsButton_Click(object sender, EventArgs e)
        {
            controlPanel.Visible = !controlPanel.Visible;
            settingsPanel.Visible = !settingsPanel.Visible;
            settingsButton.Visible = !settingsButton.Visible;

            showDetailsButton.Text = controlPanel.Visible ? "<<" : ">>";
        }



        private void CloseChannel(IScreenCastService channel)
        {
            try
            {
                var c = (IClientChannel)channel;
                if (c.State != CommunicationState.Faulted)
                {
                    c.Close();
                }
                else
                {
                    c.Abort();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //logger.Error(ex);
            }
        }

        class ComboBoxItem
        {
            public string Name { get; set; }
            public object Tag { get; set; }
        }

        public event Action OnSettingsButtonClick;
        private void settingsButton_Click(object sender, EventArgs e)
        {
            OnSettingsButtonClick?.Invoke();
        }
    }


}
