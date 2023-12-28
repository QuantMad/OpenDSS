using NetSDKCS;
using OpenPSS.Wrapper.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace OpenDSS.Wrapper.Model
{
    public class Device : ModelBase
    {
        internal ConnectionInfo connectionInfo;
        internal DeviceInfo deviceInfo;
        private string lastError;
        private DeviceConfig deviceConfig;

        public string LastError => lastError;
        public ConnectionInfo ConnectionInfo => connectionInfo;
        public DeviceInfo DeviceInfo => deviceInfo;
        public DeviceConfig DevConfig => deviceConfig;

        public Device(ConnectionInfo connInf)
        {
            connectionInfo = connInf;
            connInf.parent = this;
            OnPropertyChanged("ConnectionInfo");
        }

        private RelayCommand cmdConnect;
        public RelayCommand CmdConnect
        {
            get => cmdConnect ??
                    (cmdConnect = new RelayCommand(obj =>
                    {
                        new Task(() =>
                        {
                            if (IntPtr.Zero == connectionInfo.LoginID)
                            {
                                NET_DEVICEINFO_Ex deviceInfo_ex = new NET_DEVICEINFO_Ex();
                                connectionInfo.LoginID = NETClient.LoginWithHighLevelSecurity(
                                    connectionInfo.IP, connectionInfo.Port,
                                    connectionInfo.UserName, connectionInfo.Password,
                                    connectionInfo.LoginType, IntPtr.Zero, ref deviceInfo_ex);

                                if (IntPtr.Zero == connectionInfo.LoginID)
                                {
                                    lastError = NETClient.GetLastError();
                                    OnPropertyChanged("LastError");
                                    return;
                                }

                                deviceInfo = new DeviceInfo(this, deviceInfo_ex);
                                OnPropertyChanged("DeviceInfo");

                                /* DeviceConfig */

                                NET_USER_MANAGE_INFO_NEW uin = new NET_USER_MANAGE_INFO_NEW();
                                NETClient.QueryUserInfoNew(connectionInfo.LoginID, ref uin, 2000);
                            }
                        }).Start();
                    }));
        }

        private bool QueryFile(DateTime startTime, DateTime endTime, ref NET_RECORDFILE_INFO[] infos, ref int fileCount)
        {
            //set stream type 设置码流类型
            IntPtr pStream = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            //Marshal.StructureToPtr(EM_STREAM_TYPE.AUTO, pStream, true);
            //NETClient.SetDeviceMode(ConnectionInfo.LoginID, EM_USEDEV_MODE.RECORD_STREAM_TYPE, pStream);
            //query record file 查询录像文件

            bool ret = NETClient.QueryRecordFile(
                ConnectionInfo.LoginID, 1,
                EM_QUERY_RECORD_TYPE.ALL, startTime,
                endTime, null, ref infos,
                ref fileCount, 5000,
                false);

            if (false == ret)
            {
                return false;
            }
            return true;
        }
        /*
       */

        private RelayCommand cmdDisconnect;
        public RelayCommand CmdDisconnect
        {
            get => cmdDisconnect ??
                    (cmdDisconnect = new RelayCommand(obj =>
                    {
                        bool result = NETClient.Logout(connectionInfo.LoginID);
                        if (!result)
                        {
                            lastError = NETClient.GetLastError();
                            OnPropertyChanged("LastError");
                            return;
                        }
                        CmdStopRealPlay.Execute(null);
                        connectionInfo.LoginID = IntPtr.Zero;
                    }));
        }

        private RelayCommand StopRealPlay;
        public RelayCommand CmdStopRealPlay
        {
            get => StopRealPlay ??
                    (StopRealPlay = new RelayCommand(obj =>
                    {
                        foreach (var ch in deviceInfo.Channels)
                            ch.StopRealPlay.Execute(null);
                    }));
        }
    }
}
