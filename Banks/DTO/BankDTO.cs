using Banks.Entities;

namespace Banks.DTO
{
    public class BankDTO
    {
        public int Id { get; set; }
        public BankDTO Bank { get; set; }
        public Person Person { get; set; }
    }
}