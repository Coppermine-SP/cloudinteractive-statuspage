using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace cloudinteractive_statuspage.Models
{
    public struct CoreServiceStateItem
    {
        public string Name { get; private set; }
        public bool IsOnline { get; private set; }

        public CoreServiceStateItem(string name, bool isOnline)
        {
            Name = name;
            IsOnline = isOnline;
        }
    }
}
