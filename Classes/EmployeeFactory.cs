using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    public static class EmployeeFactory
    {

        public static Employee CreateEmployee(string login, string password, int isWorker)
        {
            Employee newEmployee = isWorker == 1 ? new Worker() : new Manager();

            newEmployee.Login = login;
            newEmployee.Password = password;

            return newEmployee;
        }
    }
}
