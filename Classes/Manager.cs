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
            Console.WriteLine("8: Просмотреть все задачи");
            Console.WriteLine("9: Просмотреть всех сотрудников");
            Console.WriteLine("10: Завершить работу");

        }

        public override void ActionsTree()
        {
            int answer = -1;

            while (answer != 10)
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

                    if (answer > 0 && answer < 11)
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
                        Console.WriteLine(" 1: Добавить сотрудника");   //сделано
                        CreateNewEmployee();
                        break;

                    case 2:
                        Console.WriteLine(" 2: Добавить задачу");   //сделано
                        CreateNewTask();
                        break;

                    case 3:
                        Console.WriteLine(" 3: Присвоить задачу сотруднику");  //сделано
                        AssignTask();
                        break;

                    case 4:
                        Console.WriteLine(" 4: Удалить задачу"); //сделано
                        DeleteTask();
                        break;

                    case 5:
                        Console.WriteLine(" 5: Удалить сотрудника"); //сделано
                        DeleteEmployee();
                        break;

                    case 6:
                        Console.WriteLine(" 6: Просмотреть историю действий работников");
                        ShowActivityLog();
                        break;

                    case 7:
                        Console.WriteLine(" 7: Просмотреть доступные действия"); //сделано
                        this.ShowActions(); 
                        break;

                    case 8:
                        Console.WriteLine(" 8: Просмотреть все задачи"); //сделано
                        ShowAllTasks();
                        break;

                    case 9:
                        Console.WriteLine(" 9: Просмотреть всех сотрудников"); //сделано
                        ShowAllWorkers();
                        break;

                    case 10:
                        Console.WriteLine(" 10: Завершить работу"); //сделано
                        break;
                }
            }
        }

        public static void CreateNewEmployee()
        {
            Console.WriteLine("Ведите новый логин: ");
            string login = Console.ReadLine();

            Console.WriteLine("Введите новый пароль: ");
            string password = Console.ReadLine();

            int isWorker = -1;
            Console.WriteLine("Новый сотрудник менеджер? \n (0 - да, 1 - нет)");
            while (!(isWorker == 0 || isWorker == 1))
            {
                string mid = Console.ReadLine();
                try
                {
                    isWorker = Convert.ToInt32(mid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Используйте только указанные выше номера вариантов. Повторите ввод числа: ");
                    continue;
                }

                if(!(isWorker == 0 || isWorker == 1))
                    Console.WriteLine("Ошибка ввода. Введите 1 или 0: ");
            }

            //через фабрику создаем работника
            var newEmployee = EmployeeFactory.CreateEmployee(0, login, password, isWorker);

            //и сохраняем в базу данных 
            DBDataAccess.SaveNewEmployee(newEmployee, isWorker);

            Console.WriteLine("Новый сотрудник добавлен.");
        }

        public static void CreateNewTask()
        {
            string taskTitle = "";
            Console.WriteLine("\n Ведите задачу: ");
            while (taskTitle == "")
            {
                taskTitle = Console.ReadLine();
                if(taskTitle =="")
                {
                    Console.WriteLine("Обнаружено пустое поле. Введите задачу еще раз: ");
                }
            }

            Console.WriteLine("\n Ведите номер связанного проекта: ");
            int projectId= -1;
            while (projectId == -1)
            {
                string mid = Console.ReadLine();
                try
                {
                    projectId = Convert.ToInt32(mid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Введите номер проекта: ");
                }
            }

            string description = "";
            Console.WriteLine("\n Ведите описание задачи: ");
            while (description == "")
            {
                description = Console.ReadLine();
                if (description == "")
                {
                    Console.WriteLine("Обнаружено пустое поле. Введите описание еще раз: ");
                }
            }



            Console.WriteLine("\n Ведите текущий статус задачи (0 - to do, 1 - in progress, 2 - done): ");
            int status = -1;
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
            }

            Console.WriteLine("Введите ID назначенного сотрудника: ");
            int workerId = -1;
            while (workerId == -1)
            {
                string mid = Console.ReadLine();
                try
                {
                    workerId = Convert.ToInt32(mid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Введите ID сотрудника: ");
                }
            }

            Task newTask = new Task(projectId, taskTitle, description, workerId, status);

            Console.WriteLine("\t Будет добавлена следующая задача: \n");
            newTask.PrintTask();

            bool flag = DBDataAccess.CheckExistingEmployee(newTask.WorkerId);

            if (flag)
            {
                DBDataAccess.SaveNewTask(newTask);
            }
          
        }

        public static void AssignTask()
        {
            List<Task> tasks = DBDataAccess.FindAllTasks();

            Console.WriteLine("\n\t Задачи: \n");
            foreach(Task task in tasks)
            {
                task.PrintTask();   
            }

            Console.WriteLine("Введите номер задачи для распределения: ");

            int taskId=-1;
  
            while (taskId == -1)
            {
                string mid = Console.ReadLine();

                try
                {
                    taskId = Convert.ToInt32(mid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Используйте только номер для указания задачи. Повторите ввод числа: ");
                }
            }

            bool flag = DBDataAccess.CheckExistingTask(taskId);
            if (!flag)
            {
                return;
            }

            Console.WriteLine("Введите номер работника для присвоения задачи: ");
            int workerId = -1;
            
            while (workerId == -1)
            {
                string mid = Console.ReadLine();

                try
                {
                    workerId = Convert.ToInt32(mid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Используйте только номер для указания работника. Повторите ввод числа: ");
                }
            }

            flag = DBDataAccess.CheckExistingEmployee(workerId);
            if (!flag)
            {
                return;
            }

            DBDataAccess.ReassignTask(taskId, workerId);
        }

        public static void DeleteTask()
        {
            Console.WriteLine("Введите номер задачи для удаления: ");

            int taskId = -1;

            while (taskId == -1)
            {
                string mid = Console.ReadLine();

                try
                {
                    taskId = Convert.ToInt32(mid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Используйте только номер для указания задачи. Повторите ввод числа: ");
                }
            }

            bool flag = DBDataAccess.CheckExistingTask(taskId);
            if (!flag)
            {
                return;
            }

            DBDataAccess.DropTask(taskId);
        }

        public static void ShowAllTasks()
        {
            List<Task> tasks = DBDataAccess.FindAllTasks();

            Console.WriteLine("\n\t Задачи: \n");
            foreach (Task task in tasks)
            {
                task.PrintTask();
            }
        }

        public static void DeleteEmployee()
        {
            Console.WriteLine("Введите ID сотрудника для удаления: ");

            int workerId = -1;

            while (workerId == -1)
            {
                string mid = Console.ReadLine();

                try
                {
                    workerId = Convert.ToInt32(mid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка ввода. Используйте только номер для указания ID сотрудника. Повторите ввод числа: ");
                }
            }

            bool flag = DBDataAccess.CheckExistingEmployee(workerId);
            if (!flag)
            {
                return;
            }

            DBDataAccess.DropUser(workerId);

        }

        public static void ShowActivityLog()
        {

        }

        public static void  ShowAllWorkers()
        {
            List<Worker> employees = DBDataAccess.FindAllWorkers();

            Console.WriteLine("\t Все работники: \n");
            foreach(Worker w in employees)
            {
                Console.WriteLine($"\t ID: {w.Id} \n\t Логин: {w.Login}\n\n");
            }
        }

        
    }
}
