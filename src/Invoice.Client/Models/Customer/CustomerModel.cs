namespace Invoice.Client.Models
{
    public class CustomerModel : DataObject
    {
        public CustomerModel() : base()
        {
            Address = new Address();
        }

        public string Name { get; set; }

        public string LegalNumber { get; set; }

        public string RegionalLegalNumber { get; set; }

        public Address Address { get; set; }
    }
}