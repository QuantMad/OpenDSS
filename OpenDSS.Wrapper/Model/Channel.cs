using NetSDKCS;
using OpenDSS.Common;
using OpenPSS.Wrapper.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenDSS.Wrapper.Model
{
    public class Channel : ModelBase
    {
        public static readonly Channel Empty = new Channel()
        {
            channelNumber = -1,
        };
        public readonly DeviceInfo Parent;

        int channelNumber;
        EM_RealPlayType realPlayType = EM_RealPlayType.EM_A_RType_Realplay;

        IntPtr realPlayID = IntPtr.Zero;
        IntPtr realPlayHandle = IntPtr.Zero;

        string lastError;

        public Channel(DeviceInfo parent = null, int num = -1)
        {
            Parent = parent;
            channelNumber = num;
            OnPropertyChanged("ChannelNumber");
        }

        public void GetTodyRecords()
        {

        }

        private bool QueryFile(DateTime startTime, DateTime endTime, ref NET_RECORDFILE_INFO[] infos, ref int fileCount)
        {
            //set stream type 设置码流类型
            //IntPtr pStream = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            //Marshal.StructureToPtr(EM_STREAM_TYPE.AUTO, pStream, true);
            //NETClient.SetDeviceMode(ConnectionInfo.LoginID, EM_USEDEV_MODE.RECORD_STREAM_TYPE, pStream);
            //query record file 查询录像文件

            bool ret = NETClient.QueryRecordFile(
                Parent.Parent.ConnectionInfo.LoginID, 1,
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

        #region properties
        public int ChannelNumber => channelNumber + 1;

        public IntPtr RealPlayHandle
        {
            get => realPlayHandle;
            private set
            {
                realPlayHandle = value;
                OnPropertyChanged("RealPayHandle");
            }
        }

        public IntPtr RealPlayID
        {
            get => realPlayID;
            private set
            {
                realPlayID = value;
                OnPropertyChanged("RealPlayID");
            }
        }

        public string LastError
        {
            get => lastError;
            set
            {
                lastError = value;
                OnPropertyChanged("LastError");
            }
        }
        #endregion properties

        #region commands
        
        public List<TimeInterval> GetIntervals()
        {
            NET_RECORDFILE_INFO[] rfi = new NET_RECORDFILE_INFO[5000];
            int fileCount = 0;

            bool ret = QueryFile(new DateTime(2023, 11, 01, 0, 0, 0), new DateTime(2023, 11, 01, 23, 59, 59), ref rfi, ref fileCount);

            if (false == ret)
            {
                Debug.WriteLine("QueryRecordFile == false");
                return null;
            }
            Debug.WriteLine("QueryRecordFile == true");

            Debug.WriteLine(fileCount);
            DateTime start = DateTime.MinValue;//= rfi[0].starttime.ToDateTime();
            DateTime end = DateTime.MinValue;// = rfi[0].endtime.ToDateTime();
            List<TimeInterval> intervals = new List<TimeInterval>();
            for (int i = 0; i < fileCount; i++)
            {
                if (start == DateTime.MinValue)
                {
                    start = rfi[i].starttime.ToDateTime();
                }
                //if ()
                if (end == DateTime.MinValue || rfi[i].starttime.ToDateTime() == end)
                {
                    end = rfi[i].endtime.ToDateTime();
                } else
                {
                    intervals.Add(new TimeInterval(start, end));
                    start = rfi[i].starttime.ToDateTime();
                    end = rfi[i].endtime.ToDateTime();
                }

                //Debug.WriteLine($"{rfi[i].starttime} -- {rfi[i].endtime}");
            }

            return intervals;
        }

        private RelayCommand startRealPlay;
        public RelayCommand StartRealPlay
        {
            get => startRealPlay ??
                    (startRealPlay = new RelayCommand(obj =>
                    {
                        StopRealPlay.Execute(this);
                        RealPlayHandle = (IntPtr)obj;
                        RealPlayID = NETClient.RealPlay(Parent.Parent.connectionInfo.LoginID, channelNumber, realPlayHandle, realPlayType);
                        /*Debug.WriteLine($"RealPlayID: {RealPlayID}");
                        Debug.WriteLine($"RealPlayHandle: {RealPlayHandle}");*/
                        if (IntPtr.Zero == realPlayID)
                        {
                            LastError = NETClient.GetLastError();
                            RealPlayHandle = IntPtr.Zero;
                            return;
                        }
                    }));
        }

        private RelayCommand stopRealPlay;
        public RelayCommand StopRealPlay
        {
            get => stopRealPlay ??
                    (stopRealPlay = new RelayCommand(obj =>
                    {
                        Debug.WriteLine(realPlayID);
                        bool err = NETClient.StopRealPlay(RealPlayID);
                        if (!err)
                        {
                            LastError = NETClient.GetLastError();
                            return;
                        }
                        RealPlayID = IntPtr.Zero;
                        RealPlayHandle = IntPtr.Zero;
                    }));
        }
        #endregion commands
    }
}
