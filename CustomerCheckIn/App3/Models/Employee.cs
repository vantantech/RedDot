namespace CustomerCheckIn.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public bool Active { get; set; }
        public string MSRCard { get; set; }
        public string SSN { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string DisplayName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Title { get; set; }
        public string PayType { get; set; }
        public int SecurityLevel { get; set; }
        public string PIN { get; set; }
        public bool Appointment { get; set; }
        public bool Sales { get; set; }
        public int SortOrder { get; set; }
        public double PayNormal { get; set; }
        public int CommissionPercent { get; set; }
        public string ImageSrc { get; set; }
        public string ReceiptStr { get; set; }
    }
}

