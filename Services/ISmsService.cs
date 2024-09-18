namespace truckPRO_api.Services
{
    public interface ISmsService
    {
        public Task<string> SendVerificationCode(string toPhoneNumber, string code);
    }
}
