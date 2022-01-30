using System;
using System.Collections.Generic;

namespace Reports.Core.Entities
{
    public class Problem
    {

        public enum ProblemState
        {
            Open,
            Active,
            Resolved,
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public ProblemState State { get; set; }
        public Sprint Sprint { get; set; }
        public Person Author { get; set; }
        public Person? Executor { get; set; }
        public List<Comment> Comments { get; set; }
    }
}