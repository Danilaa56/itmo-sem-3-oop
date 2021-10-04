using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Isu.Tools;
using IsuExtra.Services;

namespace IsuExtra.Entities
{
    public class ExtraCourse
    {
        private readonly Dictionary<string, ExtraGroup> _groups = new Dictionary<string, ExtraGroup>();

        public ExtraCourse(ExtraIsuServiceImpl extraIsuServiceImpl, MegaFaculty megaFaculty, string name)
        {
            ExtraIsuServiceImpl = extraIsuServiceImpl ?? throw new ArgumentNullException(nameof(extraIsuServiceImpl));
            MegaFaculty = megaFaculty ?? throw new ArgumentNullException(nameof(megaFaculty));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ExtraIsuServiceImpl ExtraIsuServiceImpl { get; }
        public MegaFaculty MegaFaculty { get; }
        public string Name { get; }

        public ExtraGroup AddGroup(string name, int sizeLimit)
        {
            if (name is null)
                throw new ArgumentException(nameof(name));
            if (_groups.ContainsKey(name))
                throw new IsuException("There is a group with such name on this course");
            var extraGroup = new ExtraGroup(this, name, sizeLimit);
            _groups[name] = extraGroup;
            return extraGroup;
        }

        public void DestroyGroup(string name)
        {
            if (!_groups.Remove(name))
            {
                throw new IsuException("There is no group with such id on this course");
            }
        }

        public ImmutableList<ExtraGroup> GetGroups()
        {
            return _groups.Values.ToImmutableList();
        }
    }
}