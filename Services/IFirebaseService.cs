namespace truckPRO_api.Services
{
    public interface IFirebaseService
    {
        public Task<bool> SendTestPushToManagers();
        public Task<bool> SendDriverDrivingPushToManagers(int companyId, string message, string driverFirstName, string driverLastName);

    }
}
