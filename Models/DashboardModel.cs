using Ng.Services;
using cloudinteractive_statuspage.Services;
namespace cloudinteractive_statuspage.Models
{
    public class DashboardModel
    {
        public List<NotifyItem> NotifyList = new List<NotifyItem>();
        public List<CoreServiceStateItem> CoreServiceList = new List<CoreServiceStateItem>();
        public List<ServiceStateItem> ServiceList = new List<ServiceStateItem>();
        public ConnectionStateItem ConnectionState;
        public DateTime TimeStamp { get; private set; }

        public void Vaildate()
        {
            var coreOffline = from i in CoreServiceList where !i.IsOnline select i.Name;
            var serviceOffline = from i in ServiceList where !i.IsOnline select i.Name;
            var serviceMaintenance = from i in ServiceList where i.IsMaintenance select i.Name;

            if (coreOffline.Count() != 0)
            {
               string lst = String.Join(", ", coreOffline);
               NotifyList.Insert(0, new NotifyItem(NotifyItem.NotifyType.Error, $"다음 코어 서비스에 문제가 있습니다 : '{lst}'"));
            }

            if (serviceOffline.Count() != 0)
            {
                string lst = String.Join(", ", serviceOffline);
                NotifyList.Insert(0, new NotifyItem(NotifyItem.NotifyType.Error, $"다음 서비스에 문제가 있습니다 : '{lst}'"));
            }

            if (serviceMaintenance.Count() != 0)
            {
                string lst = String.Join(", ", serviceMaintenance);
                NotifyList.Insert(0, new NotifyItem(NotifyItem.NotifyType.Warn, $"다음 서비스가 점검 중입니다 : '{lst}'"));
            }

            TimeStamp = DateTime.Now;


        }

        public string TimeStampToString()
        {
            return $"{TimeStamp.Year}.{TimeStamp.Month}.{TimeStamp.Day} ({TimeStamp.Hour}:{TimeStamp.Minute}:{TimeStamp.Second})";
        }

        public static List<NotifyItem> ConvertToNotifyItemList(List<Notify> list)
        {
            var result = new List<NotifyItem>();
            foreach (var item in list)
            {
                result.Add(new NotifyItem(item.Type == Notify.NotifyType.Info ? NotifyItem.NotifyType.Info : NotifyItem.NotifyType.Warn, item.Content));
            }
            return result;
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
