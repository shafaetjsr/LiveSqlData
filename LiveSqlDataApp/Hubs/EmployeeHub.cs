using LiveSqlDataApp.Data;
using Microsoft.AspNetCore.SignalR;

namespace LiveSqlDataApp.Hubs
{
    public class EmployeeHub:Hub
    {
        public async Task RefreshEmployees(List<Employee> employees)
        {
            await Clients.All.SendAsync("RefreshEmployees", employees);
        }
    }
}
