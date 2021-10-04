namespace Isu.Entities
{
    public class Student
    {
        public Student(int id, Group group, string name)
        {
            Id = id;
            Group = group;
            Name = name;
        }

        public string Name { get; }
        public Group Group { get; set; }
        public int Id { get; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}