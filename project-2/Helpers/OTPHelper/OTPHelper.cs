
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using project_2.Data;
using project_2.Models;
using project_2.Models.Repositories;

namespace project_2.Helpers.OTPHelper
{
    public class OTPHelper : IOTPHelper
    {
        private readonly DataDbContext context;
        IRepository<OTP> repo;

        public OTPHelper(IRepository<OTP> repo, DataDbContext context)
        {
            this.repo = repo;
            this.context = context;
        }

        public async Task<string> GenerateOTP(string email)
        {
            var code = RandomNumberGenerator.GetInt32(100000,999999).ToString();
            var entry = new OTP
            {
                Email = email,
                Code = code,
                expiry = DateTime.Now.AddMinutes(1),
            };
            repo.Add(entry);
            return code;
        }

        public async Task<bool> VerifyOTP(string email, string code)
        {
            var record = await context.OTPs.OrderBy(x=>x.CreateDate).FirstOrDefaultAsync(x=>x.Email == email && x.Code == code);
            return (record != null && record.expiry > DateTime.Now);
        }
    }
}
