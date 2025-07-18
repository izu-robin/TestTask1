using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    public class Manager : Employee
    {
        public Manager() {}

        public Manager(string log, string pass) : base(log) { }

        public static void AddManager()
        {

        }

        public static void AddWorker()
        {

        }

        public static void CreateTask()
        {

        }

        public override void ShowAllTasks()
        {
            throw new NotImplementedException();
        }
    }
}
