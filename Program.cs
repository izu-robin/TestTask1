using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography.X509Certificates;
using TestTask1.Classes;

namespace ProjectsManagingSystem
{
    class Program
    {
        enum workerStatus { Менеджер = 0, Работник = 1 }
        public static void Main()
        {
            int isWorker = -1;
            
            Employee SignedUser = new Employee();

            SignedUser = DBDataAccess.Authorize(ref isWorker);

            while (isWorker == -1)
            {
                Console.WriteLine("\n Пользователь не найден, повторите вход. \n");
                SignedUser = DBDataAccess.Authorize(ref isWorker);
            }

            Console.WriteLine($"\n\t Добро пожаловать! \n\t пользователь #{SignedUser.Id}, {SignedUser.Login}");



            //switch и вот здесь здоровый свитч со всеми командами и вот этим всем. 
            //вот это уже в него
            if (isWorker == 1)
            {
                Console.WriteLine($"\n\t Статус: сотрудник");
            }
            else
            {
                Console.WriteLine($"\n\t Статус: менеджер");
            }

        }

    }
}