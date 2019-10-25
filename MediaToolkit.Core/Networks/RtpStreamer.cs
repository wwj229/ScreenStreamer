﻿using MediaToolkit.Common;
using MediaToolkit.RTP;
using MediaToolkit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaToolkit
{


    public class RtpStreamer : IRtpSender
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public RtpStreamer(RtpSession session)
        {
            this.session = session;

        }

        private RtpSession session;
        private Socket socket;
        private IPEndPoint remoteEndpoint;


        public void Start(NetworkStreamingParams streamingParams)
        {
            try
            {
                logger.Debug("RtpStreamer::Open(...)");
                var srcAddr = streamingParams.LocalAddr;
                var srcPort = 0;//streamingParams.LocalPort;

                var localIp = IPAddress.Any;
                if (string.IsNullOrEmpty(srcAddr))
                {
                    if (IPAddress.TryParse(srcAddr, out IPAddress _localIp))
                    {
                        localIp = _localIp;
                    }
                }

                var localEndpoint = new IPEndPoint(localIp, srcPort);

                var remoteIp = IPAddress.Parse(streamingParams.RemoteAddr);
                remoteEndpoint = new IPEndPoint(remoteIp, streamingParams.RemotePort);

                logger.Debug("RtpStreamer::Open(...) " + remoteEndpoint + " " + localEndpoint);


                var bytes = remoteIp.GetAddressBytes();
                bool isMulicast = (bytes[0] >= 224 && bytes[0] <= 239);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (isMulicast)
                {
                    var ttl = streamingParams.MulticastTimeToLive;
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, ttl);
                }

                socket.Bind(localEndpoint);

                running = true;

                logger.Info("Server started " + remoteEndpoint.ToString());


                Task.Run(() =>
                {

                    while (running)
                    {
                        //if (!syncEvent.WaitOne(1000))
                        //{
                        //    continue;
                        //}

                        syncEvent.WaitOne(1000);

                        SendPackets();

                        if (!running)
                        {
                            break;
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Close();
            }
        }


        private object locker = new object();
        private AutoResetEvent syncEvent = new AutoResetEvent(false);

        private Queue<RtpPacket> packetQueue = new Queue<RtpPacket>();

        public void Push(byte[] bytes, double sec)
        {
            if (!running)
            {
                return;
            }

            if (packetQueue.Count > 1024)
            {
                packetQueue.Clear();
                logger.Warn("Buffer full drop frames...");
            }

            var packets = session.Packetize(bytes, sec);

            if (packets != null && packets.Count > 0)
            {
                foreach (var pkt in packets)
                {
                    lock (locker)
                    {
                        packetQueue.Enqueue(pkt.Clone());
                    }
                    //packetBuffer.Add(pkt);
                }
            }


            syncEvent.Set();
        }

        private void SendPackets()
        {
            if (!running)
            {
                return;
            }

            int bytesSend = 0;

            while (packetQueue.Count > 0)
            {
                if (!running)
                {
                    break;
                }

                RtpPacket pkt = null;
                lock (locker)
                {
                    pkt = packetQueue.Dequeue();
                }

                if (pkt != null)
                {
                    try
                    {
                        //var data = pkt;//.GetBytes();
                        var data = pkt.GetBytes();
                        //logger.Debug("pkt" + pkt.Sequence);
                        socket?.SendTo(data, 0, data.Length, SocketFlags.None, remoteEndpoint);
                        //socket?.BeginSendTo(data, 0, data.Length, SocketFlags.None, endpoint, null, null);
                        bytesSend += data.Length;

                        // Statistic.RtpStats.Update(MediaTimer.GetRelativeTime(), rtp.Length);
                    }
                    catch (ObjectDisposedException) { }
                }
                
            }

        }

        public void Send(byte[] bytes, double sec)
        {
            if (!running)
            {
                return;
            }

            var packets = session.Packetize(bytes, sec);

            if (packets != null && packets.Count > 0)
            {
                int bytesSend = 0;
                foreach (var pkt in packets)
                {
                    if (!running)
                    {
                        break;
                    }

                    try
                    {
                        //var data = pkt;//.GetBytes();
                        var data = pkt.GetBytes();
                        //logger.Debug("pkt" + pkt.Sequence);
                        socket?.SendTo(data, 0, data.Length, SocketFlags.None, remoteEndpoint);
                        //socket?.BeginSendTo(data, 0, data.Length, SocketFlags.None, endpoint, null, null);
                        bytesSend += data.Length;

                        // Statistic.RtpStats.Update(MediaTimer.GetRelativeTime(), rtp.Length);
                    }
                    catch (ObjectDisposedException) { }
                }
            }

        }

        private volatile bool running = false;
        public void Close()
        {

            logger.Debug("RtpStreamer::Close()");

            running = false;
            socket?.Close();

        }

    }

}
