using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography.X509Certificates;
using TestTask1.Classes;

namespace ProjectsManagingSystem
{
    class Program
    {
        public static void Main()
        {
            int isWorker = -1;
           
            var SignedUser = DBDataAccess.Authorize(ref isWorker);

            while (isWorker == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\t Пользователь не найден, повторите вход. \n");
                Console.ForegroundColor = ConsoleColor.White;
                SignedUser = DBDataAccess.Authorize(ref isWorker);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n\t Добро пожаловать! \n\t пользователь #{SignedUser.Id}, {SignedUser.Login}");
            

            Console.WriteLine(isWorker==0 ? "\t Статус: менеджер \n" : "\t Статус: сотрудник \n");
            Console.ForegroundColor = ConsoleColor.White;

            SignedUser.ShowActions();
            SignedUser.ActionsTree();
        }
    }
}