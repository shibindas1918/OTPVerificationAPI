using Microsoft.AspNetCore.Mvc;
using OTPVerificationAPI.Data;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Twilio.Types;

namespace OTPVerificationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPController : ControllerBase
    {
        private readonly OTPContext _context;

        public OTPController(OTPContext context)
        {
            _context = context;
        }

        [HttpPost("GenerateOTP")]
        public async Task<IActionResult> GenerateOTP([FromBody] string phoneNumber)
        {
            // Generate a random 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Save OTP to the database
            var otpRecord = new OTPRecord
            {
                PhoneNumber = phoneNumber,
                OTP = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5) // OTP valid for 5 minutes
            };

            _context.OTPRecords.Add(otpRecord);
            await _context.SaveChangesAsync();

            // Send OTP via SMS
            SendSMS(phoneNumber, otp);

            return Ok("OTP sent successfully!");
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerificationRequest request)
        {
            var record = _context.OTPRecords
                .FirstOrDefault(r => r.PhoneNumber == request.PhoneNumber && r.OTP == request.OTP);

            if (record == null)
                return BadRequest("Invalid OTP.");

            if (record.ExpiryTime < DateTime.UtcNow)
                return BadRequest("OTP expired.");

            return Ok("OTP verified successfully!");
        }

        private void SendSMS(string phoneNumber, string otp)
        {
            // Twilio Account SID and Auth Token
       
            try
            {
                const string AccountSid = "AC6fcfb6e3211b5ec74570edd4f1a598df";
                const string AuthToken = "ec86500cef662965581e8a36cbf42863";
                TwilioClient.Init(AccountSid, AuthToken);

                // Ensure phone number is in E.164 format
                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = "+91" + phoneNumber; // Replace "+91" with your country code if necessary
                }

                var message = MessageResource.Create(
                    body: $"Your OTP is: {otp}",
                    from: new Twilio.Types.PhoneNumber("your_twilio_phone_number"), // Replace with your Twilio number
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                Console.WriteLine($"Message sent: {message.Sid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
        }
    }

    public class OTPVerificationRequest
    {
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
    }
}
