using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace truckPro_api.Hubs
{
    public class LogHub : Hub
    {
        public async Task JoinCompanyGroup(int companyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, companyId.ToString());
        }

        public async Task LeaveCompanyGroup(int companyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, companyId.ToString());
        }

    }
}