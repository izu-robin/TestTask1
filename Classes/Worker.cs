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
        List<Task> workersTasks = new List<Task>();

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
                        Console.WriteLine("1: Просмотреть мои задачи"); // 
                        CheckMyTasks();
                        break;

                    case 2:
                        Console.WriteLine("2: Изменить статус задачи"); // 
                        ChangeStatus();
                        break;

                    case 3:
                        Console.WriteLine("3: Просмотреть доступные действия"); // 
                        this.ShowActions();
                        break;

                    case 4:
                        Console.WriteLine("4: Завершить работу"); // 
                        break;

                }
            }
        }

        public void CheckMyTasks()
        {
            workersTasks = DBDataAccess.FindPersonalTasks(this.Id);

            foreach (Task t in workersTasks)
            {
                t.PrintTask();
                Console.WriteLine();
            }

            if (workersTasks.Count == 0)
            {
                Console.WriteLine("\n\t У вас нет назначенных задач.\n");
                return;
            }
        }

        public void ChangeStatus()
        {
            CheckMyTasks();

            if(workersTasks.Count == 0)
            {
                return;
            }

            //int answer=-1;

            int tId = -1;
            bool success = false;
            int[] taskIds = new int[workersTasks.Count];

            for (int i = 0; i < workersTasks.Count; i++)
            {
                taskIds[i] = workersTasks[i].Id;
            }

            Console.WriteLine("ваши задачи: ");
            foreach (int i in taskIds)
            {
                Console.Write($"{i} ");
            }

            Console.WriteLine("\t Выберите задачу для изменения статуса: ");

            while (!success)
            {
                string line = Console.ReadLine();
                try
                {
                    tId = Convert.ToInt32(line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Используйте только указанные выше номера вариантов. Повторите ввод числа: ");
                    continue;
                }

                if (taskIds.Any( t => t == tId)) 
                {
                    success = true;
                    Console.WriteLine("у вас есть такая задача");
                }
                else
                {
                    Console.WriteLine("Некорректный выбор. Повторите ввод:");
                }
            }

            success = false;
            int status = -1;

            Console.WriteLine($"Выберите новый статус задачи {tId}");
            Console.WriteLine("0 - to do, 1 - in progress, 2 - done): ");
            while (!success)
            {
                while (!(status == 0 || status == 1 || status == 2))
                {
                    string mid = Console.ReadLine();
                    try
                    {
                        status = Convert.ToInt32(mid);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка ввода. Используйте только указанные выше номера вариантов. Повторите ввод числа: ");
                        continue;
                    }

                    if (!(status == 0 || status == 1 || status == 2))
                        Console.WriteLine("Ошибка ввода. Введите 0, 1 или 2: ");

                    if(workersTasks.Any(t => t.Id==tId && t.Status==status))
                    {
                        Console.WriteLine($"Задача #{tId} уже имеет статус {Task.ReturnStatus(status)}");
                        return;
                    }
                }
                success = true;
            }

            DBDataAccess.UpdateTaskStatus(tId, status, this.Id);



        }

    }
}
