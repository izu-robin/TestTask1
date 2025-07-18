using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    public class Manager : Employee
    {
        public Manager() { }

        public Manager(string log, string pass) : base(log) { }


        public override void ShowActions()
        {
            base.ShowActions();

            Console.WriteLine("1: Добавить сотрудника");
            Console.WriteLine("2: Добавить задачу");
            Console.WriteLine("3: Присвоить задачу сотруднику");
            Console.WriteLine("4: Удалить задачу");
            Console.WriteLine("5: Удалить сотрудника");
            Console.WriteLine("6: Просмотреть историю действий работников");
            Console.WriteLine("7: Просмотреть доступные действия");
            Console.WriteLine("8: Завершить работу");

        }

        public override void ActionsTree()
        {
            int answer = -1;

            while (answer != 8)
            {
                base.ActionsTree();
                bool success = false;

                while (!success)
                {
                    string line = Console.ReadLine();
                    try
                    {
                        answer = Convert.ToInt32(line);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка ввода. Используйте только указанные выше номера вариантов. Повторите ввод числа: ");
                        continue;
                    }

                    if (answer > 0 && answer < 9)
                    {
                        success = true;
                    }
                    else
                    {
                        Console.WriteLine("Некорректный выбор. Повторите ввод:");
                    }
                }

                switch (answer)
                {
                    case 1:
                        Console.WriteLine(" 1: Добавить сотрудника");
                        CreateNewEmployee();
                        break;

                    case 2:
                        Console.WriteLine(" 2: Добавить задачу");
                        CreateNewTask();
                        break;

                    case 3:
                        Console.WriteLine(" 3: Присвоить задачу сотруднику");
                        AssignTask();
                        break;
                    case 4:

                        Console.WriteLine(" 4: Удалить задачу");
                        DeleteTask();
                        break;

                    case 5:
                        Console.WriteLine(" 5: Удалить сотрудника");
                        DeleteEmployee();
                        break;

                    case 6:
                        Console.WriteLine(" 6: Просмотреть историю действий работников");
                        ShowActivityLog();
                        break;

                    case 7:
                        Console.WriteLine(" 7: Просмотреть доступные действия");
                        this.ActionsTree();
                        break;

                    case 8:
                        Console.WriteLine(" 8: Завершить работу");
                        break;
                }
            }
        }

        public static void CreateNewEmployee()
        {
            //через фабрику создаем работника
            string login = "";
            string password = "";
            int isWorker = 0;
            var newEmployee = EmployeeFactory.CreateEmployee(login, password, isWorker);

            //и сохраняем в базу данных 
            DBDataAccess.SaveNewEmployee(newEmployee, isWorker);
        }

        public static void CreateNewTask()
        {

        }

        public static void AssignTask()
        {

        }

        public static void DeleteTask()
        {

        }

        public static void DeleteEmployee()
        {

        }

        public static void ShowActivityLog()
        {

        }



        
    }
}
