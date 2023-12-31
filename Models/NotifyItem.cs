﻿using Ng.Services;

namespace cloudinteractive_statuspage.Models
{
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
