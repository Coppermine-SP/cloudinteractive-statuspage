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
        private ILogger _logger;
        public int RefTimeWindow { get; private set; }
        public int RefTimeout { get; private set; }
        public Int64 TotalTimeWindow { get; private set; } = 0;
        public double FailedTimeWindow { get; private set; } = 0;
        public double ServiceQuality { get; private set; }

        public delegate void ErrorCallback(StateObserver observer);

        private string _targetHostName;
        private EAddressType _addressType;

        private ErrorCallback _errorCallback;

        private Thread? _thread;
        private volatile bool _disposed;

        private Exception? _observerException;

        public string TargetHostName => _targetHostName;
        public bool Disposed => _disposed;
        public Exception? ObserverException => _observerException;
        public int ThreadId { get; private set; }

        private void _log(LogLevel level, string message)
        {
            _logger?.Log(level, $"observer-#{_thread?.ManagedThreadId ?? 0} / {message}");
        }

        public StateObserver(ILogger logger, string hostName, EAddressType addressType, ErrorCallback errorCallback, int timeWindow, int timeOut)
        {
            RefTimeWindow = timeWindow;
            RefTimeout = timeOut;
            _targetHostName = hostName;
            _addressType = addressType;
            _disposed = false;
            _errorCallback = errorCallback;
            _logger = logger;
            ThreadId = _thread?.ManagedThreadId ?? 0;
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
                _log(LogLevel.Information,$"Observer thread start. ({_targetHostName})");
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
                        Thread.Sleep(RefTimeWindow);
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

        private int _currentRetryCount = 0;
        private volatile bool _isServerOff = true;

        public bool IsServerOnline => !(_isServerOff);
        private IPAddress? _GetDnsQuery(string hostName)
        {
            IPAddress ip;
            if(IPAddress.TryParse(hostName, out ip)) return ip;

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
            using (Ping ping = new Ping())
            {
                //PingOptions pingOption = new PingOptions();
                //pingOption.DontFragment = true;
                
                PingReply reply = ping.Send(addr, RefTimeout);
                return reply.Status == IPStatus.Success;
            }
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

            TotalTimeWindow += 1;
            try
            {
                checked
                {
                    if (success)
                    {
                        if (_isServerOff)
                        {
                            FailedTimeWindow += _currentRetryCount;
                            _currentRetryCount = 0;
                            _log(LogLevel.Warning, $"{_targetHostName} is now online.");
                        }
                        _isServerOff = false;
                    }
                    else
                    {
                        if(!_isServerOff) _log(LogLevel.Warning,$"{_targetHostName} is now offline.");
                            _isServerOff = true;
                            FailedTimeWindow += 1;
                    }
                    ServiceQuality =  (1 -(FailedTimeWindow / TotalTimeWindow)) * 100.0;
                }
            }
            catch (OverflowException)
            {
                TotalTimeWindow = 0;
                FailedTimeWindow = 0;
            }
        }
    }
}
