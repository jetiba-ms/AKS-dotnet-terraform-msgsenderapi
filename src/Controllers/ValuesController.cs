using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.IO;

namespace msgsenderapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static EventHubClient eventHubClient;
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task Post()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(Environment.GetEnvironmentVariable("EventHubConnectionString"))
            {
                EntityPath = Environment.GetEnvironmentVariable("EventHubName")
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            try
            {
                string value;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    value = await reader.ReadToEndAsync();
                }
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(value)));                
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"{DateTime.Now} > Exception: {ex.Message}");
            }

            await eventHubClient.CloseAsync();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
