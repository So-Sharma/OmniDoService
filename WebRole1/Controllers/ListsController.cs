using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class ListsController : ApiController
    {
        [Route("api/lists")]
        public async Task<IHttpActionResult> Get()
        {
            var query = new TableQuery<ListEntity>().Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, "1"));

            var userDocumentEntities = await TableStorage.Lists.ExecuteQueryAsync(query);

            List<ToDoList> todoLists = new List<ToDoList>();

            foreach (ListEntity entity in userDocumentEntities)
            {
                todoLists.Add(new ToDoList(entity));
            }

            return Ok(todoLists);
        }

        [Route("api/lists/{listId}")]
        public string Get(int listId)
        {
            // TODO:
            return "value";
        }

        public async Task<bool> ifIdExists(string id)
        {
            var query = new TableQuery<ListEntity>().Where(TableQuery.GenerateFilterCondition(TableStorage.RowKey, QueryComparisons.Equal, id));
            var listId = await TableStorage.Lists.ExecuteQueryAsync(query);

            if (listId.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [Route("api/lists")]
        public async Task<IHttpActionResult> Post(ToDoList list)
        {
            list.Id = Guid.NewGuid().ToString();

            var insertOperation = TableOperation.Insert(new ListEntity(list));

            var result = TableStorage.Lists.Execute(insertOperation);

            return Created(string.Empty, new ToDoList((ListEntity)result.Result));
        }

        [Route("api/lists/{listId}")]
        public void Put(int listId, [FromBody]string value)
        {
        }

        public void Delete(int id)
        {
        }

        [Route("api/lists/{listId}/tasks")]
        public async Task<IHttpActionResult> Get(string listId)
        {
            // TODO:

            bool idExists = await ifIdExists(listId);

            if (idExists)
            {
                var query = new TableQuery<TaskEntity>().Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, listId));

                var userTaskEntities = await TableStorage.Tasks.ExecuteQueryAsync(query);

                List<ToDoTask> todoTasks = new List<ToDoTask>();

                foreach (TaskEntity entity in userTaskEntities)
                {
                    todoTasks.Add(new ToDoTask(entity));
                }

                return Ok(todoTasks);
            }
            else
            {
                return NotFound();
            }

            //var query = new TableQuery<ListEntity>().Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, "1"));

            //return Ok(new [] {"task_1", "task_2"});
        }

        [Route("api/lists/{listId}/tasks")]
        public async Task<IHttpActionResult> Post(string listId, ToDoTask task)
        {
            bool idExists = await ifIdExists(listId);

            if (idExists)
            {
                task.Id = Guid.NewGuid().ToString();

                var insertOperation = TableOperation.Insert(new TaskEntity(listId, task));

                var result = TableStorage.Tasks.Execute(insertOperation);

                return Created(string.Empty, new ToDoTask((TaskEntity)result.Result));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
