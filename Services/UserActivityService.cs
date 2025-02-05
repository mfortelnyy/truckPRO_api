using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using truckPRO_api.Data;
using truckPRO_api.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace truckPRO_api.Services
{
    public class UserActivityService : BackgroundService
    {
    }
}