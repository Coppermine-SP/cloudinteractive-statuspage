using Ng.Services;
namespace cloudinteractive_statuspage.Models
{
    public class DashboardModel
    {
        public List<NotifyItem> NotifyList = new List<NotifyItem>();
        public ConnectionStateItem ConnectionState;
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

    public struct ConnectionStateItem
    {
        public string IP;
        public UserAgent Agent;

        public ConnectionStateItem(string ip, UserAgent agent)
        {
            IP = ip == "::1" ? "127.0.0.1" : ip;
            Agent = agent;
        }
    }
}
