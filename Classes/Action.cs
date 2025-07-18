using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    public  class Action
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int WorkerId { get; set; }
        public string WorkerLogin { get; set; }
        public int TaskId { get; set; }
        public string ActionTime { get; set; }

        private int _newStatus = 0;
        int NewStatus 
        {
           get => _newStatus;

           set
           {
                _newStatus = (value >= 0 && value < 3) ? value : _newStatus;
           }
        }

        public Action() { }
        public Action(int id, int workerId, int taskId, string actionTime, int newStatus)
        {
            Id=id;
            WorkerId=workerId;
            TaskId=taskId;
            ActionTime=actionTime;
            NewStatus=newStatus;
        }

        public void PrintAction()
        {
            Console.WriteLine($"\n\t {this.ActionTime} \n\t Работник [{this.WorkerId}, {this.WorkerLogin}] изменил статус задачи #{this.TaskId} на \"{Task.ReturnStatus(this.NewStatus)}\"\n\t ");
        }
    }
}
