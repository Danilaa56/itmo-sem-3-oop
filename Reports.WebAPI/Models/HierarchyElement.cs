using System;
using System.Collections.Generic;

namespace Reports.WebAPI.Models
{
    public class HierarchyElement
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<HierarchyElement> Workers { get; set; }
    }
}