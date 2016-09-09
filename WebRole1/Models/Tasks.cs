using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class ToDoTasks
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ToDoTasks(TaskEntity task)
        {
            this.Id = task.RowKey;
            this.Name = task.Name;
        }

        public ToDoTasks()
        {
        }
    }

    public class TaskEntity : TableEntity
    {
        public TaskEntity(Guid listId, ToDoTasks task)
        {
            this.PartitionKey = listId.ToString();
            this.RowKey = task.Id;
            this.Name = task.Name;
        }

        public TaskEntity()
        {
        }

        public string Name { get; set; }
        public string Id { get; set; }


    }
}