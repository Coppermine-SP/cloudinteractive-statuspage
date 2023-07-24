namespace cloudinteractive_statuspage.Models
{
    public class DashboardModel
    {
        public List<NotifyItem> NotifyList = new List<NotifyItem>();
    }

    public struct NotifyItem
    {
        public enum NotifyType
        {
            Info,
            Warn,
            Error
        };

        public NotifyType Type;
        public string Content;

        public NotifyItem(NotifyType type, string content)
        {
            Type = type;
            Content = content;
        }
    }
}
