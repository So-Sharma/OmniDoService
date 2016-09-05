using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{


    public class ToDoList
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ToDoList(ListEntity list)
        {
            this.Id = list.RowKey;
            this.Name = list.Name;
        }

        public ToDoList()
        {
        }
    }

    public class ListEntity : TableEntity
    {
        public ListEntity(ToDoList list)
        {
            this.PartitionKey = "1";
            this.RowKey = list.Id;
            this.Name = list.Name;
           }

        public ListEntity()
        {
        }

        public string Name { get; set; }
        public string Id { get; set; }

 
    }
}