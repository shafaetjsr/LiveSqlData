using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using LiveSqlDataApp.Data;
using Newtonsoft.Json;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using Microsoft.AspNetCore.SignalR;
using LiveSqlDataApp.Hubs;

namespace LiveSqlDataApp.Services
{
    
    public class EmployeeService
    {
        private string _connectionString = "";

        private readonly SqlTableDependency<Employee> _dependency;
        private readonly IHubContext<EmployeeHub> _context;

        public EmployeeService(IHubContext<EmployeeHub>context)
        {
            _connectionString = "Server=.;Database=ConpanyDB;User ID=sa;Password=****;Trusted_Connection=False;MultipleActiveResultSets=true;";
            _dependency = new SqlTableDependency<Employee>(_connectionString, "Employee");
            _dependency.OnChanged += Change;
            _dependency.Start();
            _context = context;
        }

        private async void Change(object sender, RecordChangedEventArgs<Employee> e)
        {
            var employee = await GetAllEmployee();
            await _context.Clients.All.SendAsync("RefreshEmployees", employee);
        }

        public DataTable ExecuteSQLDatatable(string SQLQuery)
        {
            SqlConnection obcon = new SqlConnection(_connectionString);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand();
            da.SelectCommand.Connection = obcon;
            SqlCommand cmd = da.SelectCommand;
            cmd.CommandText = SQLQuery;
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public async Task<List<Employee>>GetAllEmployee()
        {
            List<Employee> list = new List<Employee>();

            string sql = "SELECT * FROM dbo.Employee";
            DataTable dt = ExecuteSQLDatatable(sql);
            if(dt != null && dt.Rows.Count> 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                list = JsonConvert.DeserializeObject<List<Employee>>(json);
            }

            return list;

        }




    }


}
