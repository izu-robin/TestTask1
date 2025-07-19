using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace TestTask1.Classes
{
    class Task
    {
        public int Id { get; set; } = 0;
        public int ProjectId { get; set; }
        public string Title { get; set; } = "N/A";
        public string Description { get; set; } = "N/A";

        public int WorkerId { get; set; }

        private int _status = 0;
        public int Status 
        { 
            get => _status;
            set { _status = (value >= 0 && value < 3) ? value : _status; } 
        }

        public Task() { }

        public Task(int projectId, string title, string description, int workerId, int status)
        {
            //Id = id;
            ProjectId = projectId;
            Title = title;
            Description = description;
            WorkerId = workerId;
            Status = status;
        }


        public static  string ReturnStatus(int index)
        {
            switch (index)
            {
                case 0:
                default:
                    return "To do";
                    
                case 1:
                    return "In progress";

                case 2:
                    return "Done";

            }
        }

        public void PrintTask()
        {   
            if(this.Id==0)
            {
                Console.WriteLine($"\n\t Задача: {this.Title}\n\t Связанный проект: {this.ProjectId}\n\t Назначенный работник: {this.WorkerId}\n\t Детали: {this.Description}\n\t Статус: {ReturnStatus(this.Status)}\n");
            }
            else
            {
                Console.WriteLine($"\n\t Задача #{this.Id}: {this.Title}\n\t Связанный проект: {this.ProjectId}\n\t Назначенный работник: {this.WorkerId}\n\t Детали: {this.Description}\n\t Статус: {ReturnStatus(this.Status)}\n");
            }
        }
    }
}
