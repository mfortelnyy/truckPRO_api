namespace truckPRO_api.Services
{
    public interface IEmailService
    {
      Task<bool> SendWelcomeEmailAsync(string receiverEmail);
      Task<bool> ReSendVerification(string receiverEmail, string verificationCode);

    }
}
