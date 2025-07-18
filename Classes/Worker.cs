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
        List<Task> usersTasks = new List<Task>();

        public Worker() {  }
        public Worker(string log, string pass) : base(log) { }

        public override void ShowActions()
        {
            base.ShowActions();

            Console.WriteLine("1: Просмотреть мои задачи");
            Console.WriteLine("2: Изменить статус задачи");
            Console.WriteLine("3: Просмотреть доступные действия");
            Console.WriteLine("4: Завершить работу");

        }

        public override void ActionsTree()
        {
            int answer = -1;

            while (answer != 4)
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

                    if (answer > 0 && answer < 5)
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
                        Console.WriteLine("1: Просмотреть мои задачи");
                        CheckMyTasks();
                        break;
                    case 2:
                        Console.WriteLine("2: Изменить статус задачи");
                        ChangeStatus();
                        break;

                    case 3:
                        Console.WriteLine("3: Просмотреть доступные действия");
                        this.ShowActions();
                        break;

                    case 4:
                        Console.WriteLine("4: Завершить работу");
                        break;

                }
            }
        }

        public void CheckMyTasks()
        {
            List<Task> myTasks = DBDataAccess.FindPersonalTasks(this.Id);

            foreach (Task t in myTasks)
            {
                t.PrintTask();
                Console.WriteLine();
            }
        }

        public static void ChangeStatus()
        {

        }

    }
}
