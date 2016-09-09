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

        [Route("api/lists")]
        public async Task<IHttpActionResult> Post(ToDoList list)
        {
            /*var user = await GetUser();

            if (user == null)
            {
                // Unexpected. User doesn't exist. Should be invalid operation as user object is not posted yet.
                return BadRequest();
            }*/

            list.Id = Guid.NewGuid().ToString();

            // Ignore permission specified in the initial post.
            //document.Permissions = new[] { new Permission(user.Phone, Role.Owner.ToString()) };

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

            var query = new TableQuery<ListEntity>().Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, "1"));

            return Ok(new [] {"task_1", "task_2"});
        }

        [Route("api/lists/{listId}/tasks/{taskId}")]
        public async Task<IHttpActionResult> Get(string listId, string taskId)
        {
            // TODO:

            var query = new TableQuery<ListEntity>().Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, "1"));

            return Ok("task_1");
        }
    }
}
