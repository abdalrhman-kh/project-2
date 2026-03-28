namespace project_2.Models
{
    public class OTP: BaseEntity
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime expiry {  get; set; }
    }
}
