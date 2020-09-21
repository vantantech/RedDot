namespace SalonMobile.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public int PreferPhone { get; set; }
        public bool AllowSMS { get; set; }
        public string Company { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DisplayName { get; set; }
        public string Phone1 { get; set; }
        public string PhoneFormatted { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public int NumberofVisit { get; set; }
        public int NumberofReferral { get; set; }
        public double TierReward { get; set; }
        public double TotalPurchased { get; set; }
        public double RewardEarned { get; set; }
        public double RewardTotal { get; set; }
        public double RewardUsed { get; set; }
        public double RewardBalance { get; set; }
        public double UsableBalance { get; set; }
        public double RewardCredit { get; set; }
        public string ImageSrc { get; set; }
    }


}

