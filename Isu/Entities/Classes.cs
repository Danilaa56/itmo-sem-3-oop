using System;

namespace Isu.Entities
{
    public class Classes
    {
        public Classes(DateTime start, DateTime finish, StudentsGroup group, string tutor)
        {
            Start = start;
            Finish = finish;
            Group = group ?? throw new ArgumentNullException(nameof(group));
            Tutor = tutor ?? throw new ArgumentNullException(nameof(tutor));
        }

        public DateTime Start { get; }
        public DateTime Finish { get; }
        public StudentsGroup Group { get; }
        public string Tutor { get; }

        public bool NotIntersecting(Classes other)
        {
            return (Start <= other.Start && Finish <= other.Start) || (Start >= other.Finish && Finish >= other.Finish);
        }
    }
}