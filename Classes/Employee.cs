using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    public class Employee
    {
        public int Id { get; set; }
        public string Login { get; set; }
        //public string Password { get; set; }

        public virtual void ShowAllTasks()
        {

        }

        public Employee() {}

        public Employee(string Login)
        {
            Login = Login;
            //Password = Password;
        }


    }
}
