using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1.Classes
{
    class Task
    {
        public int Id { get; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public Task() { }

        public Task(int projectId, string title, string description, int status)
        {
            //Id = id;
            ProjectId = projectId;
            Title = title;
            Description = description;
            Status = status;
        }

        enum TaskStatus 
        { 
            To_do=1,
            In_Progress = 2,
            Done = 3
        }
    }
}
