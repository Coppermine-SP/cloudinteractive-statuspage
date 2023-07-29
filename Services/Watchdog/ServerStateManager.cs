namespace cloudinteractive_statuspage.Services.Watchdog
{
    public class ServerStateManager
    {
        private Dictionary<string, StateObserver> _observerTable;
        private int _pingTimeoutMs;
        private int _offDecidingTimeMs;
        private int _pollingIntervalMs;

        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollingIntervalMs">ping 요청할 간격, ms 단위</param>
        /// <param name="pingTimeoutMs">ping 타임아웃, ms 단위</param>
        /// <param name="offDecidingTimeMs">ping 실패 시, 실패가 어느정도 지속되어야 서버가 off로 됐는지 판단할 때 쓰는 값. ms 단위</param>
        public ServerStateManager(int pollingIntervalMs, int pingTimeoutMs, int offDecidingTimeMs)
        {
            _pollingIntervalMs = pollingIntervalMs;
            _offDecidingTimeMs = offDecidingTimeMs;
            _pingTimeoutMs = pingTimeoutMs;

            _observerTable = new Dictionary<string, StateObserver>();
            _disposed = false;
        }

        public ServerStateManager AddObserver(ILogger logger, string key, string address, StateObserver.ErrorCallback errorCallback, EAddressType addressType)
        {
            if (_observerTable.ContainsKey(key))
                throw new Exception("duplicated key.");

            _observerTable.Add(key, new StateObserver(logger, address, addressType, errorCallback, _pollingIntervalMs, _pingTimeoutMs, _offDecidingTimeMs));

            return this;
        }

        public StateObserver? GetObserver(string key)
        {
            if (_observerTable.ContainsKey(key)) return _observerTable[key];

            return null;
        }

        public StateObserver[] GetObserverAll() => _observerTable.Values.ToArray();

        public async Task StartAsync()
        {
            if (_disposed) return;
            if (_observerTable.Count == 0) return;

            List<Task> tasks = new List<Task>(_observerTable.Count);
            foreach (var observer in _observerTable.Values)
            {
                tasks.Add(observer.StartAsync());
            }

            await Task.WhenAll(tasks);
        }

        public void Release()
        {
            if (_disposed) return;
            foreach (var observer in _observerTable.Values)
            {
                observer.Stop();
            }

            _disposed = true;
            _observerTable.Clear();
        }
    }
}
