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
        public string Password { get; set; }

        public virtual void ShowActions()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\t Выберите действие: \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public virtual void ActionsTree()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n Введите номер действия: ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public Employee() {}

        public Employee(string login)
        {
            Login = login;
        }
    }
}
