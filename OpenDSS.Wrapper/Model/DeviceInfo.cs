using NetSDKCS;
using System.Collections.Generic;

namespace OpenDSS.Wrapper.Model
{
    public class DeviceInfo
    {
        public readonly Device Parent;

        private readonly List<Channel> channels = new List<Channel>();

        NET_DEVICEINFO_Ex deviceInfo;

        public DeviceInfo(Device parend, NET_DEVICEINFO_Ex deviceInfo)
        {
            this.Parent = parend;
            this.deviceInfo = deviceInfo;

            for (int i = 0; i < deviceInfo.nChanNum; i++)
                channels.Add(new Channel(this, i));
        }

        public string SerialNumber => deviceInfo.sSerialNumber;
        public int DiskNum => deviceInfo.nDiskNum;
        public EM_NET_DEVICE_TYPE DVRType => deviceInfo.nDVRType;
        public int ChanNum => deviceInfo.nChanNum;
        public byte ByLimitLoginTime => deviceInfo.byLimitLoginTime;
        public int LockLeftTime => deviceInfo.nLockLeftTime;
        public List<Channel> Channels => channels;
    }
}
