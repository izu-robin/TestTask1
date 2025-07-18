using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    public static class EmployeeFactory
    {

        public static Employee CreateEmployee(int id, string login, string password, int isWorker)
        {
            Employee newEmployee = isWorker == 1 ? new Worker() : new Manager();
            //если нужно будет потом добавить другой тип работника, то можно переписать это через switch,
            //но в случае 2 вариантов достаточно тернарного оператора.  

            newEmployee.Id = id;
            newEmployee.Login = login;
            newEmployee.Password = password;

            return newEmployee;
        }
    }
}
