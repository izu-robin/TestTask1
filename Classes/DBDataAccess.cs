using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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
            Employee found = new Employee();
            int success = 0;

            string login = "";
            string password = "";

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

            string sql = "SELECT Employees.isWorker, Employees.id, Employees.login FROM Employees WHERE login = @login AND password = @password";
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

                        found.Id = reader.GetInt32(1);
                        found.Login = reader.GetString(2);

                    }

                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return found;
        }

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
