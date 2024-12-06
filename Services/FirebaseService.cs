using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using truckPRO_api.Services;

namespace truckPro_api.Services
{
    public class FirebaseService (IUserService userService) : IFirebaseService
    {
        private readonly IUserService _userService = userService;

        public async Task<bool> SendTestPushToManagers()
        {
            var FCMTokensManagers = await _userService.GetManagerFcmTokensAsync(1);
            var message = new MulticastMessage
            {
                Tokens = FCMTokensManagers, 
                Notification = new Notification
                {
                    Title = "Test Notification", 
                    Body = $"This is a test message." 
                }
            };

            try
            {
                // send the message with cloud msg
            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                
                // check if sent
                if (response.FailureCount > 0)
                {
                    Console.WriteLine($"Failed to send {response.FailureCount} messages.");
                    for (int i = 0; i < response.Responses.Count; i++)
                    {
                        var responseItem = response.Responses[i];
                        if (!responseItem.IsSuccess)
                        {
                            string failedToken = FCMTokensManagers[i];
                            Console.WriteLine($"Failed to send to token: {failedToken}");
                            Console.WriteLine($"Error: {responseItem.Exception?.Message}");
                            Console.WriteLine($"Error Code: {responseItem.Exception?.HResult}");
        
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Successfully sent all messages.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending push notification: {ex.Message}");
                return false;
            }
        }
        
    }
}