
namespace truckPRO_api.Services
{
    public class SmsService : ISmsService
    {
        Task<string> ISmsService.SendVerificationCode(string toPhoneNumber, string code)
        {
            throw new NotImplementedException();
        }
    }
}
