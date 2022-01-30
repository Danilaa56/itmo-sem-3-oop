using System;

namespace Reports.Core.Entities
{
    public class Token
    {
        public Guid Id { get; set; }
        public Guid TokenValue { get; set; }
        public string KeyWord { get; set; }
    }
}