using System;

namespace Reports.Core.Entities
{
    public class Sprint
    {
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
    }
}