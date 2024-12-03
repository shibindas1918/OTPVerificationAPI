using Microsoft.EntityFrameworkCore;

namespace OTPVerificationAPI.Data
{
    public class OTPContext : DbContext
    {
        public OTPContext(DbContextOptions<OTPContext> options) : base(options) { }

        public DbSet<OTPRecord> OTPRecords { get; set; }
    }

    public class OTPRecord
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
