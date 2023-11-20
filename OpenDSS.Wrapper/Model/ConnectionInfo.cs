using NetSDKCS;
using System;

namespace OpenDSS.Wrapper.Model
{
    public class ConnectionInfo : ModelBase
    {
        internal Device parent;
        string ip;
        ushort port = 37777;
        string userName;
        string password;
        EM_LOGIN_SPAC_CAP_TYPE loginSpecCapType = EM_LOGIN_SPAC_CAP_TYPE.TCP;
        IntPtr loginID = IntPtr.Zero;
        IntPtr pCapParam;
        //public bool IsConnected { get; private set; } = false;

        public ConnectionInfo(Device parent = null)
        {
            this.parent = parent;
        }

        #region callbacks
        public void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            // Offline
        }

        public void ReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            // Online
        }
        #endregion callbacks

        public bool IsConnected => LoginID != IntPtr.Zero;
        #region properties
        public string IP
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged("IP");
            }
        }

        public ushort Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged("Port");
            }
        }

        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string Password
        {
            get => password;
            set
            {
                OnPropertyChanged("Password");
                password = value;
            }
        }

        public EM_LOGIN_SPAC_CAP_TYPE LoginType
        {
            get => loginSpecCapType;
            set
            {
                OnPropertyChanged("LoginType");
                loginSpecCapType = value;
            }
        }

        public IntPtr LoginID
        {
            get => loginID;
            internal set
            {
                loginID = value;
                OnPropertyChanged("LoginID");
            }
        }

        #endregion properties
    }
}
