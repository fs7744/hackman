using System.Net.NetworkInformation;

namespace Hackman
{
    public class IdGenerator
    {
        private readonly long twepoch = 1588435200000L;
        private const int workerIdBits = 10;
        private const int timestampBits = 41;
        private const int sequenceBits = 12;
        private const int maxWorkerId = ~(-1 << workerIdBits);
        private long workerId;
        private long timestampAndSequence;
        private readonly long timestampAndSequenceMask = ~(-1L << (timestampBits + sequenceBits));

        public IdGenerator(long workerId = -1)
        {
            InitTimestampAndSequence();
            InitWorkerId(workerId);
        }

        private void InitTimestampAndSequence()
        {
            long timestamp = GetNewestTimestamp();
            long timestampWithSequence = timestamp << sequenceBits;
            this.timestampAndSequence = timestampWithSequence;
        }

        private void InitWorkerId(long workerId)
        {
            if (workerId < 0)
            {
                workerId = GenerateWorkerId();
            }
            if (workerId > maxWorkerId || workerId < 0)
            {
                string message = string.Format("worker Id can't be greater than {0} or less than 0", maxWorkerId);
                throw new ArgumentException(message);
            }
            this.workerId = workerId << (timestampBits + sequenceBits);
        }

        public long NextId()
        {
            WaitIfNecessary();
            long next = Interlocked.Increment(ref timestampAndSequence);
            long timestampWithSequence = next & timestampAndSequenceMask;
            return workerId | timestampWithSequence;
        }

        private void WaitIfNecessary()
        {
            long currentWithSequence = timestampAndSequence;
            long current = currentWithSequence >> sequenceBits;
            long newest = GetNewestTimestamp();
            if (current >= newest)
            {
                Thread.Sleep(5);
            }
        }

        private long GetNewestTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - twepoch;
        }

        public static long GenerateWorkerId()
        {
            try
            {
                return GenerateWorkerIdBaseOnMac();
            }
            catch (Exception)
            {
                return GenerateRandomWorkerId();
            }
        }

        public static long GenerateWorkerIdBaseOnMac()
        {
            IEnumerable<NetworkInterface> all = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in all)
            {
                bool isLoopback = networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback;
                //bool isVirtual = networkInterface.;
                //if (isLoopback || isVirtual)
                if (isLoopback)
                {
                    continue;
                }
                byte[] mac = networkInterface.GetPhysicalAddress().GetAddressBytes();
                return ((mac[4] & 0B11) << 8) | (mac[5] & 0xFF);
            }
            throw new Exception("no available mac found");
        }

        public static long GenerateRandomWorkerId()
        {
            return Random.Shared.NextInt64(maxWorkerId + 1);
        }
    }
}