using NetSDKCS;
using OpenDSS.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OpenDSS.Wrapper
{
    public class DevicesProvider : ModelBase
    {
        private static fDisConnectCallBack m_DisConnectCallBack;
        private static fHaveReConnectCallBack m_ReConnectCallBack;
        private static fSnapRevCallBack m_SnapRevCallBack;

        private readonly List<Device> devices = new List<Device>();
        public List<Device> Devices => devices;

        public DevicesProvider()
        {
            m_DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
            m_ReConnectCallBack = new fHaveReConnectCallBack(ReConnectCallBack);
            try
            {
                NETClient.Init(m_DisConnectCallBack, IntPtr.Zero, null);
                NETClient.SetAutoReconnect(m_ReConnectCallBack, IntPtr.Zero);
                NETClient.SetSnapRevCallBack(m_SnapRevCallBack, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Process.GetCurrentProcess().Kill();
            }

            ReadDevicesLocally();

            OnPropertyChanged("Device");
            foreach (var device in devices)
                device.CmdConnect.Execute(null);
        }

        private void ReadDevicesLocally()
        {
            string path = ".\\devices.txt";
            foreach (var line in File.ReadAllLines(path))
            {
                string[] data = line.Split(' ');

                Devices.Add(new Device(new ConnectionInfo()
                {
                    IP = data[0],
                    UserName = data[1],
                    Password = data[2],
                }));
            }
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {

        }

        private void ReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {

        }

        /*private bool QueryFile(DateTime startTime, DateTime endTime, ref NET_RECORDFILE_INFO[] infos, ref int fileCount)
        {
            //set stream type 设置码流类型
            //EM_STREAM_TYPE streamType = (EM_STREAM_TYPE)play_stream_comboBox.SelectedIndex + 1;
            
        }*/
    }
}
