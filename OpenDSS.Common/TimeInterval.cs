using System;

namespace OpenDSS.Common
{
    public struct TimeInterval
    {
        DateTime start;
        public DateTime Start
        {
            get => start; set => start = value;
        }
        
        DateTime end;
        public DateTime End
        {
            get => end; set => end = value;
        }

        public TimeSpan Length => End - Start;

        public TimeInterval(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
