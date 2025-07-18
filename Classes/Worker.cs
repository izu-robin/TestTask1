using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    public class Worker : Employee
    {
        public Worker() {  }
        public Worker(string log, string pass) : base(log) { }


        public static void CheckMyTasks()
        {

        }

        public static void ChangeStatus()
        {

        }

        public override void ShowAllTasks()
        {
            throw new NotImplementedException();
        }
    }
}
