namespace project_2.Helpers.OTPHelper
{
    public interface IOTPHelper
    {
        Task<string> GenerateOTP(string email);
        Task<bool> VerifyOTP(string email, string code);
    }
}
