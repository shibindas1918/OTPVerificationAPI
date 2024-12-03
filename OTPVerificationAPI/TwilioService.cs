using Twilio;
using Twilio.Rest.Api.V2010.Account;

public class TwilioService
{
    private const string AccountSid = "your_account_sid"; // Replace with your Account SID
    private const string AuthToken = "your_auth_token";   // Replace with your Auth Token

    public static void SendOtp(string toPhoneNumber, string otp)
    {
        try
        {
            TwilioClient.Init(AccountSid, AuthToken);

            // Ensure phone number is in E.164 format
            if (!toPhoneNumber.StartsWith("+"))
            {
                toPhoneNumber = "+91" + toPhoneNumber; // Replace "+91" with your country code if necessary
            }

            var message = MessageResource.Create(
                body: $"Your OTP is: {otp}",
                from: new Twilio.Types.PhoneNumber("your_twilio_phone_number"), // Replace with your Twilio number
                to: new Twilio.Types.PhoneNumber(toPhoneNumber)
            );

            Console.WriteLine($"Message sent: {message.Sid}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
