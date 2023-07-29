using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace cloudinteractive_statuspage.Services.Watchdog
{
    public enum EAddressType
    {
        DNS,
        IPv4
    }
    public class StateObserver
    {
        public delegate void ErrorCallback(StateObserver observer);

        private string _targetHostName;
        private EAddressType _addressType;
        private int _pingTimeout;
        private int _offDecidingTime;
        private int _pollingIntervalMs;

        private ErrorCallback _errorCallback;

        private Thread? _thread;
        private volatile bool _disposed;

        private Exception? _observerException;

        public string TargetHostName => _targetHostName;
        public bool Disposed => _disposed;
        public Exception? ObserverException => _observerException;

        public StateObserver(string hostName, EAddressType addressType, ErrorCallback errorCallback, int pollingInterval, int pingTimeout, int offDecidingTime)
        {
            _targetHostName = hostName;
            _addressType = addressType;
            _pollingIntervalMs = pollingInterval;
            _offDecidingTime = offDecidingTime;
            _pingTimeout = pingTimeout;
            _disposed = false;
            _errorCallback = errorCallback;
        }

        ~StateObserver()
        {
            if (_disposed) return;

            Dispose();
        }

        public async Task StartAsync()
        {
            await Task.Run(() =>
            {
                _thread = new Thread(ThreadRun);
                _thread.IsBackground = true;
                _thread.Start();
            });
        }

        public void Stop()
        {
            if (_disposed) return;

            Dispose();
        }

        private void Dispose()
        {
            _disposed = true;
            _thread?.Interrupt();
            _thread = null;
        }

        private void ThreadRun()
        {
            while (true)
            {
                try
                {
                    while (!_disposed)
                    {
                        Poll();
                        Thread.Sleep(_pollingIntervalMs);
                    }
                }
                catch (ThreadInterruptedException) { return; }
                catch (Exception ex)
                {
                    _observerException = ex;
                    _errorCallback(this);
                    Dispose();
                    break;
                }
            }
        }

        private bool _firstRequest = true;
        private DateTime _serverBeginTimestamp;
        private DateTime _serverLastTimestamp;
        private double _totalUptime = 0.0;
        private volatile bool _isServerOff = true;

        public bool IsServerOnline => !(_isServerOff);
        public double SLA { get; private set; }
        private IPAddress? _GetDnsQuery(string hostName)
        {
            IPHostEntry entry = Dns.GetHostEntry(hostName);

            if (entry.AddressList.Length == 0) return null;

            return entry.AddressList[0];
        }

        private IPAddress? _GetAddress(string hostName)
        {
            return IPAddress.Parse(hostName);
        }
        private (string, int?) ParseAddressAndPort(string rawHostName)
        {
            int index = rawHostName.IndexOf(':');

            if (index == -1)
                return (rawHostName, null);

            string sAddr = rawHostName.Substring(0, index);
            string sPort = rawHostName.Substring(index + 1);

            int nPort = int.Parse(sPort);
            return (sAddr, nPort);
        }
        private bool CheckStateWithPing(IPAddress addr)
        {
            Ping ping = new Ping();
            PingOptions pingOption = new PingOptions();
            pingOption.DontFragment = true;
            PingReply reply = ping.Send(addr, _pingTimeout);

            return reply.Status == IPStatus.Success;
        }
        private bool CheckStateWithTelnet(IPAddress addr, int port)
        {
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(addr, port);
                    return true;
                }
            }
            catch (SocketException)
            {
                return false;
            }
        }
        private void Poll()
        {
            IPAddress? targetAddr;

            (string, int?) tuple = ParseAddressAndPort(_targetHostName);

            if (_addressType == EAddressType.DNS)
                targetAddr = _GetDnsQuery(tuple.Item1);
            else
                targetAddr = _GetAddress(tuple.Item1);

            bool success = false;

            if (targetAddr != null)
            {
                success = tuple.Item2.HasValue ? CheckStateWithTelnet(targetAddr, tuple.Item2.Value) : CheckStateWithPing(targetAddr);
            }

            try
            {
                checked
                {
                    if (success)
                    {
                        // 만약, 최초 핑 요청일 때 타임스탬프 기록 
                        if (_firstRequest)
                        {
                            _firstRequest = false;
                            _serverLastTimestamp = _serverBeginTimestamp = DateTime.Now;
                        }

                        var time = DateTime.Now - _serverLastTimestamp;
                        _totalUptime = time.TotalMilliseconds + _totalUptime;

                        _serverLastTimestamp = DateTime.Now;


                        _isServerOff = false;
                    }
                    else
                    {
                        // 일정시간 응답이 없었다면 서버 off 판정
                        var current = DateTime.Now;
                        var timeSpan = current - _serverLastTimestamp;

                        if (timeSpan.Milliseconds >= _offDecidingTime)
                        {
                            _isServerOff = true;
                        }
                    }


                    if (_firstRequest)
                    {
                        SLA = 0.0;
                    }
                    else
                    {
                        var totalTime = DateTime.Now - _serverBeginTimestamp;
                        SLA = _totalUptime / totalTime.TotalMilliseconds * 100.0;
                    }
                }
            }
            catch (OverflowException)
            {
                _firstRequest = true;
                _serverLastTimestamp = _serverBeginTimestamp = DateTime.Now;
                _totalUptime = 0;
            }
        }
    }
}
