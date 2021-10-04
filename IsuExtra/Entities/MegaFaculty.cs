using System;

namespace IsuExtra.Entities
{
    public class MegaFaculty
    {
        public MegaFaculty(string name, string groupNamePattern)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GroupNamePattern = groupNamePattern ?? throw new ArgumentNullException(nameof(groupNamePattern));
        }

        public string Name { get; }
        public string GroupNamePattern { get; }
    }
}