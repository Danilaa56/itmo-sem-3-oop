namespace Shops.Entities
{
    public class Person
    {
        public Person(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public decimal Money { get; set; }
    }
}