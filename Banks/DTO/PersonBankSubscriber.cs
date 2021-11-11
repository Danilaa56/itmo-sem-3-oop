using Banks.Entities;

namespace Banks.DTO
{
    public class PersonBankSubscriber
    {
        public int Id { get; set; }
        public Person Person { get; set; }
        public Bank Bank { get; set; }
    }
}