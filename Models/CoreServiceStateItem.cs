namespace cloudinteractive_statuspage.Models
{
    public struct CoreServiceStateItem
    {
        public string Name { get; set; }
        public bool IsOnline { get; set; }

        public CoreServiceStateItem(string name, bool isOnline)
        {
            Name = name;
            IsOnline = isOnline;
        }
    }
}
