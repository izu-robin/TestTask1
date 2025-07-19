using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace TestTask1.Classes
{

    class DBDataAccess
    {
        private static readonly Lazy<DBDataAccess> instance = new Lazy<DBDataAccess>(() => new DBDataAccess());

        public static DBDataAccess Instance => instance.Value;
        public string ConnectionString { get; private set; }

        private DBDataAccess()
        {
            ConnectionString = @"URI=file:ManagingSystemDB.db";
        }

        public static Employee Authorize( ref int isWorker)
        {
            int success = 0;

            string login = "";
            string password = "";
            int id=-1;

            while (success != 1)
            {
                Console.WriteLine("Введите логин: ");
                login = Console.ReadLine();

                Console.WriteLine("Введите пароль: ");
                password = Console.ReadLine();

                if (login == "" || password == "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\t Обнаружены пустые поля, повторите ввод. \n");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                success = 1;
            }

            string sql = "SELECT Employees.isWorker, Employees.id, Employees.login, Employees.password FROM Employees WHERE login = @login AND password = @password";
            string connectionString = Instance.ConnectionString;

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection);

                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int employeeClass = reader.GetInt32(0);
                        if (employeeClass == 1)
                        {
                            isWorker = 1;
                        }
                        else if(employeeClass == 0)
                        {
                            isWorker = 0;
                        }

                        id = reader.GetInt32(1);
                        login=reader.GetString(2);
                        password=reader.GetString(3);
                    }

                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Пользователь не авторизирован: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            var found = EmployeeFactory.CreateEmployee( id, login, password, isWorker);

            return found;
        }

        public static void SaveNewEmployee(Employee e, int isWorker)
        {
            string sql = "INSERT INTO Employees (login, password, isWorker) VALUES (@login, @password, @isWorker)";
            string connectionString = Instance.ConnectionString;

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@login", e.Login);
                        command.Parameters.AddWithValue("@password", e.Password);
                        command.Parameters.AddWithValue("@isWorker", isWorker);

                        command.ExecuteNonQuery();
                    }

                    SQLiteConnection.Close();
                }
            }
            catch(Exception ex)
            {
                if (ex.Message == "constraint failed\r\nUNIQUE constraint failed: Employees.login")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Такой логин уже занят. Выберите другой логин и повторите операцию. ");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Не удалось сохранить нового сотрудника в базу данных: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n Новый сотрудник добавлен. \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static bool CheckExistingEmployee(int id)
        {
            string connectionString = Instance.ConnectionString;

            string sql = "SELECT Employees.* FROM Employees WHERE Employees.id=@id AND Employees.isWorker=1";
            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        var reader = command.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Не найден работник с этим ID. Проверьте данные и повторите ввод.");
                            Console.ForegroundColor = ConsoleColor.White;
                            return false;
                        }
                        reader.Close(); //вот из-за него (без него), открытого но не очищенного и незакрытого было database closed

                    }
                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return true;
        }

        public static bool CheckExistingTask(int id)
        {
            string connectionString = Instance.ConnectionString;

            string sql = "SELECT Tasks.* FROM Tasks WHERE Tasks.Id=@Id";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        var reader = command.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Не найдена задача с этим ID. Проверьте данные и повторите ввод.");
                            return false;
                        }
                        reader.Close(); 

                    }
                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return true;
        }

        public static void SaveNewTask(Task t)
        {
            string connectionString = Instance.ConnectionString;

            string sql = "INSERT INTO Tasks (ProjectId, Title, Description, Status, WorkerId) VALUES (@ProjectId, @Title, @Description, @Status, @WorkerId)";
            
            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@ProjectId", t.ProjectId);
                        command.Parameters.AddWithValue("@Title", t.Title);
                        command.Parameters.AddWithValue("@Description", t.Description);
                        command.Parameters.AddWithValue("@Status", t.Status);
                        command.Parameters.AddWithValue("@WorkerId", t.WorkerId);

                        command.ExecuteNonQuery();
                    }
                   SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\t Задача сохранена. \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static List<Worker> FindAllWorkers()
        {
            List<Worker> employeesList = new List<Worker>();

            string sql = "SELECT Employees.id, Employees.login FROM Employees WHERE Employees.isWorker=1";
            string connectionString = Instance.ConnectionString;

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection);

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Worker e = new Worker();
                        e.Id= reader.GetInt32(0);
                        e.Login= reader.GetString(1);

                        employeesList.Add(e);
                    }

                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return employeesList;
        }
        public static List<Task> FindAllTasks()
        {
            List<Task> taskList = new List<Task>();

            string sql = "SELECT Tasks.* FROM Tasks";
            string connectionString = Instance.ConnectionString;

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection);

                    using var reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        Task t = new Task();
                        t.Id = reader.GetInt32(0);
                        t.ProjectId=reader.GetInt32(1);
                        t.Title= reader.GetString(2);   
                        t.Description= reader.GetString(3);
                        t.Status=reader.GetInt32(4);

                        if(reader.IsDBNull(5))
                        {
                            t.WorkerId = 0;
                        }
                        else 
                        {
                            t.WorkerId = reader.GetInt32(5);
                        }
                            

                        taskList.Add(t);
                    }
                   
                    SQLiteConnection.Close();
                }
            }
            catch(Exception ex) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return taskList;
        }
 
        public static void ReassignTask(int taskId, int workerId)
        {
            string connectionString = Instance.ConnectionString;
            string sql = "UPDATE Tasks SET workerId = @workerId WHERE Id = @Id";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@workerId", workerId);
                        command.Parameters.AddWithValue("@Id", taskId);

                        command.ExecuteNonQuery();

                    }
                    SQLiteConnection.Close();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Задача обновлена.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void DropTask(int taskId)
        {
            string connectionString = Instance.ConnectionString;
            string sql = "DELETE FROM Tasks WHERE Id = @Id";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@Id", taskId);

                        command.ExecuteNonQuery();

                    }
                    SQLiteConnection.Close();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Задача удалена.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void DropUser(int workerId)
        {
            string connectionString = Instance.ConnectionString;
            string sql = "DELETE FROM Employees WHERE id = @id";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@id", workerId);

                        command.ExecuteNonQuery();

                    }
                    SQLiteConnection.Close();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Сотрудник удален.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static List<Task> FindPersonalTasks(int id)
        {
            List<Task>  tasksList= new List<Task>();

            string connectionString = Instance.ConnectionString;
            string sql = "SELECT Tasks.*, Employees.login FROM Tasks INNER JOIN Employees ON Tasks.workerId=Employees.id WHERE Tasks.WorkerId= @Id ";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                           Task t= new Task();
                            t.Id = reader.GetInt32(0);
                            t.ProjectId=reader.GetInt32(1);
                            t.Title=reader.GetString(2);
                            t.Description=reader.GetString(3);  
                            t.Status=reader.GetInt32(4);
                            t.WorkerId=reader.GetInt32(5);

                            tasksList.Add(t);
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return tasksList;
        }

        public static void UpdateTaskStatus(int taskId, int status, int workerId)
        {
            string connectionString = Instance.ConnectionString;
            string sql = "UPDATE Tasks SET Status = @Status WHERE Id = @Id";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@Id", taskId);

                        command.ExecuteNonQuery();

                    }
                    SQLiteConnection.Close();

                    Console.WriteLine("\nЗадача обновлена.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            LogAction(new Action(workerId, taskId, DateTime.Now.ToString(), status));
        }

        public static void LogAction(Action a)
        {
            string connectionString = Instance.ConnectionString;

            string sql = "INSERT INTO Actions (workerId, taskId, NewStatus, ActionTime) VALUES (@workerId, @taskId, @NewStatus, @ActionTime)";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@workerId", a.WorkerId);
                        command.Parameters.AddWithValue("@taskId", a.TaskId);
                        command.Parameters.AddWithValue("@NewStatus", a.NewStatus);
                        command.Parameters.AddWithValue("@ActionTime", a.ActionTime);

                        command.ExecuteNonQuery();
                    }
                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Задача #{a.TaskId} получила статус {Task.ReturnStatus(a.NewStatus)}");
            Console.WriteLine($"Изменение сохранено в истории действий.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ReadLog()
        {
            List<Action> actionLog = new List<Action>();

            string connectionString = Instance.ConnectionString;
            string sql = "SELECT Actions.id, Actions.ActionTime, Actions.taskId, Tasks.Title, Actions.newStatus, Actions.workerId, Employees.login FROM (Employees JOIN Tasks ON Employees.id=Tasks.workerId) JOIN Actions ON Tasks.id=Actions.taskId";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(connectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        using var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Action a = new Action();
                            a.Id = reader.GetInt32(0);
                            a.ActionTime = reader.GetString(1);
                            a.TaskId = reader.GetInt32(2);
                            a.Title=reader.GetString(3);
                            a.NewStatus = reader.GetInt32(4);

                            if(reader.IsDBNull(5))
                            {
                                a.WorkerId = 0;
                            }
                            else
                            {
                                a.WorkerId = reader.GetInt32(5);
                            }

                            if(reader.IsDBNull(6))
                            {
                                a.WorkerLogin = " Удаленный пользователь";
                            }
                            else
                            {
                                a.WorkerLogin = reader.GetString(6);
                            }

                            actionLog.Add(a);
                        }

                        reader.Close();
                    }

                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка соединения с базой данных: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if(actionLog.Count == 0)
            {
                Console.WriteLine("\n\t История действий пуста. \n");
                return;
            }    

            foreach(Action a in actionLog)
            {
                a.PrintAction();
            }
        }
    }
}
