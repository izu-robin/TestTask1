using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
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
            //Console.WriteLine("\t связь с базой установлена");
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
                    Console.WriteLine("\t Обнаружены пустые поля, повторите ввод. \n");
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
                Console.WriteLine($"Пользователь не авторизирован: {ex.Message}");
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
                    Console.WriteLine("Такой логин уже занят. Выберите другой логин и повторите операцию. ");
                }
                else
                {
                    Console.WriteLine($"Не удалось сохранить нового сотрудника в базу данных: {ex.Message}");
                }
            }
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
                            Console.WriteLine("Не найден работник с этим ID. Проверьте данные и повторите ввод.");
                            return false;
                        }
                        reader.Close(); //вот из-за него (без него), открытого но не очищенного и незакрытого было database closed

                    }
                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
            }
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
                        t.WorkerId=reader.GetInt32(5);

                        taskList.Add(t);
                    }
                   
                    SQLiteConnection.Close();
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return taskList;

        }
        //--------------------------------------------------------------------------------------------------
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
                    //SQLiteConnection.Close();

                    Console.WriteLine("Задача обновлена.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

                    Console.WriteLine("Задача удалена.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

                    Console.WriteLine("Сотрудник удален.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void FindAllActivity()
        {
            string connectionString = Instance.ConnectionString;
            string sql = "SELECT Actions.* FROM Actions";

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
                            //забиваем полученное по полям объекта
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
            }


            return tasksList;

        }



        //------------------------------------------ удалить потом надо, пока что справочныйф угол ------------------------------------
        static void AddNewManager() //сырое из первых попыток, но по итогу работает. 
            {        //абсолютная ссылка
                     //string ConnectionString = @"Data Source=L:\Programming\Codin\C#\TestTask1\ManagingSystemDB.db";

                // блять это абсолютная ссылка на дебажную версию
                string baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                // и строим абсолютную ссылку с подобранной директорией
                string dbPath = Path.Combine(baseDirectory, "ManagingSystemDB.db");

                //string ConnectionString = @"Data Source=ManagingSystemDB.db;FailIfMissing=True";
                //{dbPath}


                string dbfile = @"URI=file:ManagingSystemDB.db";     // еще такой вариант, тоже все в дебажную версию
                SQLiteConnection connection = new SQLiteConnection(dbfile);


                string addManager = @"INSERT INTO Managers (login, password) 
                                  VALUES (@Login, @Password)";



                try //вот сюда включать всё что работает с базой данных
                {
                    using (SQLiteConnection SQLiteConnection = new SQLiteConnection(dbfile))
                    {
                        SQLiteConnection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(addManager, SQLiteConnection))
                        {
                            command.Parameters.AddWithValue("@Login", "admin1");
                            command.Parameters.AddWithValue("@Password", "passwoord");

                            int rowsChanged = command.ExecuteNonQuery();
                            Console.WriteLine($"Изменена {rowsChanged} строка");
                        }

                        Console.WriteLine("Creation succeeded!");
                        SQLiteConnection.Close();
                        Console.Read();
                    }
                }
                catch (SQLiteException ex)
                {
                    if (ex.Message == "constraint failed\r\nUNIQUE constraint failed: Managers.login")
                        Console.WriteLine("Такой логин уже занят. Повторите ввод: ");
                    else
                        Console.WriteLine($"Ошибка подключения базы данных: {ex.Message}");

                }
            }
        


        




    }
}
