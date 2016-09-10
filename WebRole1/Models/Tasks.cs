using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class ToDoTask
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ToDoTask(TaskEntity task)
        {
            this.Id = task.RowKey;
            this.Name = task.Name;
        }

        public ToDoTask()
        {
        }
    }

    public class TaskEntity : TableEntity
    {
        public TaskEntity(string listId, ToDoTask task)
        {
            this.PartitionKey = listId;
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