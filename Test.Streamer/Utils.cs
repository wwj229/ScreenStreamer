﻿using MediaToolkit.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TestStreamer
{

    public class ServerSettings
    {
        public string StreamName = "";

        public string NetworkIpAddress = "0.0.0.0";
        public int CommunicationPort = -1;

        public TransportMode TransportMode = TransportMode.Tcp;
        public bool IsMulticast = false;
        public string MutlicastAddress = "239.0.0.1";
    }

    public class AudioStreamSettings
    {
        public bool Enabled = false;
        public string SessionId = "";
        public NetworkSettings NetworkParams = null;
        public AudioEncoderSettings EncodingParams = null;
        public AudioCaptureSettings CaptureParams = null;
    }


    public class VideoStreamSettings
    {
        public bool Enabled = false;
        public string SessionId = "";
        public NetworkSettings NetworkParams = null;
        public VideoCaptureDescription CaptureDescription = null;
        public VideoEncoderSettings EncodingParams = null;

    }

    public class NetUtils
    {
        public static IEnumerable<int> GetFreePortRange(ProtocolType protocolType, int portsCount,
                        int leftBound = 49152, int rightBound = 65535, IEnumerable<int> exceptPorts = null)
        {
            var totalRange = Enumerable.Range(leftBound, rightBound - leftBound + 1);

            IPGlobalProperties ipProps = IPGlobalProperties.GetIPGlobalProperties();

            IEnumerable<System.Net.IPEndPoint> activeListeners = null;
            if (protocolType == ProtocolType.Udp)
            {
                activeListeners = ipProps.GetActiveUdpListeners()
                    .Where(listener => listener.Port >= leftBound && listener.Port <= rightBound);
            }
            else if (protocolType == ProtocolType.Tcp)
            {
                activeListeners = ipProps.GetActiveTcpListeners()
                    .Where(listener => listener.Port >= leftBound && listener.Port <= rightBound);
            }

            //foreach (var listner in activeListeners) 
            //{
            //    Debug.WriteLine(listner);
            //}

            if (activeListeners == null) return null;

            //Список свободных портов  
            var freePorts = totalRange.Except(activeListeners.Select(listener => listener.Port));
            if (exceptPorts != null)
            {
                freePorts = freePorts.Except(exceptPorts);
            }

            return freePorts;
        }
    }


}
