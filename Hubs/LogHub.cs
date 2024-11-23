using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FirebaseAdmin.Messaging;
using truckPRO_api.Services;

namespace truckPro_api.Hubs
{
    public class LogHub (IUserService userService) : Hub
    {
        private readonly IUserService _userService = userService;

        public async Task JoinCompanyGroup(int companyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, companyId.ToString());
        }

        public async Task LeaveCompanyGroup(int companyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, companyId.ToString());
        }

        //notify managers when driver creates a log
        public async Task NotifyManagers(int companyId, string logDetails)
        {
            //send SignalR message to all managers of the company (through the SignalR group)
            await Clients.Group(companyId.ToString()).SendAsync("ReceiveNotification", $"Driver has started a new log: {logDetails}");

            //send a push notification to all managers using Firebase
            await SendPushNotificationToManagers(companyId, logDetails);
        }

        //send a push notification via firebase admin sdk
        private async Task SendPushNotificationToManagers(int companyId, string messageText)
        {
            //get all device tokens of the manager
            List<string> fcmTokens = await _userService.GetManagerFcmTokensAsync(companyId);

            if (fcmTokens.Count > 0)
            {
              
                var messages = new MulticastMessage
                {
                    Tokens = fcmTokens,
                    Notification = new Notification
                    {
                        Title = "New Driving Log Created",
                        Body = messageText
                    },
                };
                
                try
                {
                    var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(messages);
                    Console.WriteLine($"Successfully sent {response.SuccessCount} messages.");

                    if (response.FailureCount > 0)
                    {
                        Console.WriteLine($"Failed to send {response.FailureCount} messages.");

                        for (int i = 0; i < response.Responses.Count; i++)
                        {
                            var responseItem = response.Responses[i];
                            if (!responseItem.IsSuccess)
                            {
                                string failedToken = fcmTokens[i];
                                Console.WriteLine($"Failed to send to token: {failedToken}");
                                Console.WriteLine($"Error: {responseItem.Exception?.Message}");

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending push notification: {ex.Message}");
                }
            }              
        }
    }
}
