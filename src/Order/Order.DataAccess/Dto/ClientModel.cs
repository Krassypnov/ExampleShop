

namespace Order.DataAccess.Dto
{
    public class ClientModel
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool Delivery { get; set; }
        public bool IsClientInfoFull()
            => !(string.IsNullOrWhiteSpace(Name)
              || string.IsNullOrWhiteSpace(Address)
              || string.IsNullOrWhiteSpace(Phone));
    }
}
