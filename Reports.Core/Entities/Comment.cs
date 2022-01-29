using System;

namespace Reports.Core.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Person Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Content { get; set; }
    }
}