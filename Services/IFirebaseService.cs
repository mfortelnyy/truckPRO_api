namespace truckPRO_api.Services
{
    public interface IFirebaseService
    {
        public Task<bool> SendTestPushToManagers();
    }
}
