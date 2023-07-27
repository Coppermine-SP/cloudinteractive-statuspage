using Ng.Services;
namespace cloudinteractive_statuspage.Models
{
    public class DashboardModel
    {
        public List<NotifyItem> NotifyList = new List<NotifyItem>();
        public List<CoreServiceStateItem> CoreServiceList = new List<CoreServiceStateItem>();
        public ConnectionStateItem ConnectionState;

        public void Vaildate()
        {
            var core_offline = from i in CoreServiceList where !i.IsOnline select i.Name;

            if (core_offline.Count() != 0)
            {
               string lst = String.Join(", ", core_offline);
               NotifyList.Insert(0, new NotifyItem(NotifyItem.NotifyType.Error, $"다음 코어 서비스에 문제가 있습니다 : '{lst}'"));
            }

            
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
