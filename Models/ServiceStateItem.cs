namespace cloudinteractive_statuspage.Models
{
    public struct ServiceStateItem
    {
        public string Name { get; private set; }
        public string SubName { get; private set; }
        public bool IsOnline { get; private set; }
        public bool IsMaintenance { get; private set; }
        public float SLA { get; set; }

        public ServiceStateItem(string name, string subName, bool isOnline, bool isMaintenance, float sla)
        {
            Name = name;
            IsOnline = isOnline;
            IsMaintenance = isMaintenance;
            SLA = sla;
            SubName = subName;

        }
    }
}
