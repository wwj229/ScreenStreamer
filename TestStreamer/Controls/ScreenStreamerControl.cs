﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaToolkit;
using System.Net.NetworkInformation;
using MediaToolkit.Common;

namespace TestStreamer.Controls
{
    public partial class ScreenStreamerControl : UserControl
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ScreenStreamerControl()
        {
            InitializeComponent();

            LoadScreenItems();
            LoadTransportItems();
            LoadEncoderItems();

            UpdateControls();

        }

        private readonly MainForm mainForm = null;

        private bool isStreaming = false;
        private ScreenStreamer videoStreamer = null;
        private ScreenSource screenSource = null;


        private StatisticForm statisticForm = new StatisticForm();
        private PreviewForm previewForm = null;


        private void startButton_Click(object sender, EventArgs e)
        {
            logger.Debug("startButton_Click(...)");

            var localAddr = "0.0.0.0";

            
            //var ipInfo = GetCurrentIpAddrInfo();
            //if (ipInfo != null)
            //{
            //    localAddr = ipInfo.Address?.ToString();
            //}


            int fps = (int)fpsNumeric.Value;


            bool showMouse = showMouseCheckBox.Checked;


            bool aspectRatio = aspectRatioCheckBox.Checked;
            var top = (int)srcTopNumeric.Value;
            var left = (int)srcLeftNumeric.Value;
            var right = (int)srcRightNumeric.Value;
            var bottom = (int)srcBottomNumeric.Value;

            int width = right - left;
            int height = bottom - top;

            var srcRect = new Rectangle(left, top, width, height); //currentScreen.Bounds;
            //var srcRect = currentScreen.Bounds;

            var _destWidth = (int)destWidthNumeric.Value;
            var _destHeight = (int)destHeightNumeric.Value;

            var destSize = new Size(_destWidth, _destHeight);

            //if (aspectRatio)
            //{
            //    var ratio = srcRect.Width / (double)srcRect.Height;
            //    int destWidth = destSize.Width;
            //    int destHeight = (int)(destWidth / ratio);
            //    if (ratio < 1)
            //    {
            //        destHeight = destSize.Height;
            //        destWidth = (int)(destHeight * ratio);
            //    }

            //    destSize = new Size(destWidth, destHeight);

            //    logger.Info("New destionation size: " + destSize);
            //}



            screenSource = new ScreenSource();
            ScreenCaptureParams captureParams = new ScreenCaptureParams
            {
                SrcRect = srcRect,
                DestSize = destSize,
                CaptureType = CaptureType.DXGIDeskDupl,
                //CaptureType = CaptureType.Direct3D,
                //CaptureType = CaptureType.GDI,
                Fps = fps,
                CaptureMouse = showMouse,
                AspectRatio = aspectRatio,
            };

            screenSource.Setup(captureParams);

            var cmdOptions = new CommandLineOptions();
            cmdOptions.IpAddr = addressTextBox.Text;
            cmdOptions.Port = (int)portNumeric.Value;

            NetworkStreamingParams networkParams = new NetworkStreamingParams
            {

                LocalAddr = localAddr,
                LocalPort = cmdOptions.Port,

                RemoteAddr = cmdOptions.IpAddr,
                RemotePort = cmdOptions.Port,
            };

            VideoEncodingParams encodingParams = new VideoEncodingParams
            {
                Width = destSize.Width, // options.Width,
                Height = destSize.Height, // options.Height,
                FrameRate = cmdOptions.FrameRate,
                EncoderName = "libx264", // "h264_nvenc", //
            };

            videoStreamer = new ScreenStreamer(screenSource);

            videoStreamer.Setup(encodingParams, networkParams);


            statisticForm.Location = currentScreen.Bounds.Location;
            statisticForm.Start();

            var captureTask = screenSource.Start();
            var streamerTask = videoStreamer.Start();


            isStreaming = true;

            UpdateControls();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("stopButton_Click(...)");

            if (screenSource != null)
            {
                screenSource.Close();
            }

            if (videoStreamer != null)
            {
                videoStreamer.Close();

            }

            if (statisticForm != null)
            {
                statisticForm.Stop();
                statisticForm.Visible = false;
            }

            if (previewForm != null && !previewForm.IsDisposed)
            {
                previewForm.Close();
                previewForm = null;
            }

            isStreaming = false;

            UpdateControls();

        }


        private void previewButton_Click(object sender, EventArgs e)
        {
            if (previewForm != null && !previewForm.IsDisposed)
            {
                previewForm.Visible = !previewForm.Visible;
            }
            else
            {

                previewForm = new PreviewForm();
                previewForm.Setup(screenSource);
                var pars = screenSource.CaptureParams;

                var title = "Src" + pars.SrcRect + "->Dst" + pars.DestSize + " Fps=" + pars.Fps +  " Ratio=" + pars.AspectRatio;

                previewForm.Text = title;

                previewForm.Visible = true;
            }
        }

        private void screensUpdateButton_Click(object sender, EventArgs e)
        {
            LoadScreenItems();
        }

        private BindingList<ComboBoxItem> screenItems = null;
        private void LoadScreenItems()
        {
            var screens = Screen.AllScreens.Select(s => new ComboBoxItem { Name = s.DeviceName, Tag = s }).ToList();
            //screens.Add(new ComboBoxItem
            //{
            //    Name = "_SelectRegion",
            //    Tag = null
            //});

            screenItems = new BindingList<ComboBoxItem>(screens);
            screensComboBox.DisplayMember = "Name";
            screensComboBox.DataSource = screenItems;
        }

        private Screen currentScreen = null;
        private void screensComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            currentScreen = GetCurrentScreen();

            if (currentScreen != null)
            {
                var rect = currentScreen.Bounds;

                srcTopNumeric.Value = rect.Top;
                srcLeftNumeric.Value = rect.Left;
                srcRightNumeric.Value = rect.Right;
                srcBottomNumeric.Value = rect.Bottom;
            }

        }

        private void screensComboBox_SelectedValueChanged_1(object sender, EventArgs e)
        {
            currentScreen = GetCurrentScreen();

            if (currentScreen != null)
            {
                var rect = currentScreen.Bounds;

                srcTopNumeric.Value = rect.Top;
                srcLeftNumeric.Value = rect.Left;
                srcRightNumeric.Value = rect.Right;
                srcBottomNumeric.Value = rect.Bottom;
            }
        }

        private Screen GetCurrentScreen()
        {
            Screen screen = null;
            var obj = screensComboBox.SelectedItem;
            if (obj != null)
            {
                var item = obj as ComboBoxItem;
                if (item != null)
                {
                    var tag = item.Tag;
                    if (tag != null)
                    {
                        screen = tag as Screen;
                    }
                }
            }
            return screen;
        }




        private void LoadTransportItems()
        {

            var items = new List<TransportMode>
            {
                TransportMode.Tcp,
                TransportMode.Udp,

            };
            transportComboBox.DataSource = items;
        }

        private void LoadEncoderItems()
        {
            var items = new List<VideoEncoderMode>
            {
                VideoEncoderMode.H264,
                VideoEncoderMode.JPEG,
            };

            encoderComboBox.DataSource = items;
    
        }

        private void UpdateControls()
        {
            this.settingPanel.Enabled = !isStreaming;

            this.startButton.Enabled = !isStreaming;
            this.previewButton.Enabled = isStreaming;

            this.stopButton.Enabled = isStreaming;


            //this.fpsNumeric2.Enabled = !ServiceHostOpened;
            //this.inputSimulatorCheckBox2.Enabled = !ServiceHostOpened;
            //this.screensComboBox2.Enabled = !ServiceHostOpened;
            //this.screensUpdateButton2.Enabled = !ServiceHostOpened;

        }


    }
}
