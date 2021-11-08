using Banks.Entities;

namespace Banks
{
    public class Context
    {
        public CentralBank CentralBank { get; } = new CentralBank();
        public PeopleRegistry PeopleRegistry { get; } = new PeopleRegistry();
    }
}