namespace cloudinteractive_statuspage.Models
{
    public class ServiceStateItem
    {
        public string Name { get; set; }
        public short SLA { get; set; }
        public bool isOnline { get; set; }
    }
}
