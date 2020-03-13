namespace ManufacturingInventory.Infrastructure.Model.Interfaces{
    public interface IContact {
        int Id { get; set; }
        string LastName { get; set; }
        string Phone { get; set; }
        string Title { get; set; }
        string Website { get; set; }
        string Address { get; set; }
        string Comments { get; set; }
        string CountryCode { get; set; }
        string Email { get; set; }
        string Extension { get; set; }
        string Fax { get; set; }
        string FirstName { get; set; }
    }
}