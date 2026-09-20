using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BoTech.HttpClientHelper.Tests.TestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/test/json
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public IActionResult GetJson()
        {
            return Ok(new TestDto()
            {
                Name = "Florian",
                Age = 19,
                Birthday = new DateTime(2030, 1, 1)
            });
        }

        // GET: api/test/string
        [HttpGet("[action]")]
        public IActionResult GetString()
        {
            return Content("This is a test string", "text/plain");
        }
        /// <summary>
        /// Returns a small file / byte stream (for HttpGetFile tests)
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public IActionResult GetFile()
        {
            var bytes = Encoding.UTF8.GetBytes("This is a test file content.");
            return File(bytes, "application/octet-stream", "testfile.txt");
        }

        /// <summary>
        /// Expects JSON in the body and returns a JSON echo with a generated ID (201 Created).
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult PostJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> PostHttpContentAndGetJson()
        {
            using var reader = new StreamReader(Request.Body);
            string data = await reader.ReadToEndAsync();
            
            return Ok(new TestDto()
            {
                Name = data,
                Age = 19,
                Birthday = new DateTime(2030, 1, 1)
            });
        }
        [HttpPost("[action]")]
        public IActionResult PostJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpPost("[action]")]
        public IActionResult Post()
        {
            return Ok();
        }
        
        
        [HttpPut("[action]")]
        public IActionResult PutJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpPut("[action]")]
        public async Task<IActionResult> PutHttpContentAndGetJson()
        {
            using var reader = new StreamReader(Request.Body);
            string data = await reader.ReadToEndAsync();
            
            return Ok(new TestDto()
            {
                Name = data,
                Age = 19,
                Birthday = new DateTime(2030, 1, 1)
            });
        }
        [HttpPut("[action]")]
        public IActionResult PutJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpPut("[action]")]
        public IActionResult Put()
        {
            return Ok();
        }
        
        
        [HttpPatch("[action]")]
        public IActionResult PatchJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpPatch("[action]")]
        public async Task<IActionResult> PatchHttpContentAndGetJson()
        {
            using var reader = new StreamReader(Request.Body);
            string data = await reader.ReadToEndAsync();
            
            return Ok(new TestDto()
            {
                Name = data,
                Age = 19,
                Birthday = new DateTime(2030, 1, 1)
            });
        }
        [HttpPatch("[action]")]
        public IActionResult PatchJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpPatch("[action]")]
        public IActionResult Patch()
        {
            return Ok();
        }
        

        [HttpDelete("[action]")]
        public IActionResult DeleteJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteHttpContentAndGetJson()
        {
            using var reader = new StreamReader(Request.Body);
            string data = await reader.ReadToEndAsync();
            
            return Ok(new TestDto()
            {
                Name = data,
                Age = 19,
                Birthday = new DateTime(2030, 1, 1)
            });
        }
        [HttpDelete("[action]")]
        public IActionResult DeleteJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpDelete("[action]")]
        public IActionResult Delete()
        {
            return Ok();
        }
    }
}
