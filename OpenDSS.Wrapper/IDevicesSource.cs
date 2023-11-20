using OpenDSS.Wrapper.Model;
using System.Collections.Generic;

namespace OpenDSS.Wrapper
{
    public interface IDevicesSource
    {
        List<Device> GetDevices();
        void EditDevice();
        bool AddDevice();
        bool DropDevice();
    }
}
