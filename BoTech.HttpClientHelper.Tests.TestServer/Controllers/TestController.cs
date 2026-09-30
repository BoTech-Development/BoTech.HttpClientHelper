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
        public ActionResult<TestDto> GetJson()
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
        public ContentResult GetString()
        {
            return Content("This is a test string", "text/plain");
        }
        /// <summary>
        /// Returns a small file / byte stream (for HttpGetFile tests)
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public FileContentResult GetFile()
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
        public ActionResult<TestDto> PostJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<TestDto>> PostHttpContentAndGetJson()
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PostJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpPost("[action]")]
        public ActionResult Post()
        {
            return Ok();
        }
        
        
        [HttpPut("[action]")]
        public ActionResult<TestDto> PutJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpPut("[action]")]
        [ProducesResponseType(typeof(TestDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TestDto>> PutHttpContentAndGetJson()
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PutJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpPut("[action]")]
        public ActionResult Put()
        {
            return Ok();
        }
        
        
        [HttpPatch("[action]")]
        public ActionResult<TestDto> PatchJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpPatch("[action]")]
        public async Task<ActionResult<TestDto>> PatchHttpContentAndGetJson()
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PatchJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpPatch("[action]")]
        public ActionResult Patch()
        {
            return Ok();
        }
        

        [HttpDelete("[action]")]
        public ActionResult<TestDto> DeleteJsonAndGetJson([FromBody] TestDto data)
        {
            data.Name = "FloBo";
            return Ok(data);
        }
        [HttpDelete("[action]")]
        public async Task<ActionResult<TestDto>> DeleteHttpContentAndGetJson()
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteJson([FromBody] TestDto data)
        {
            if(data.Name == "Florian")
                return Ok();
            return BadRequest();
        }
        [HttpDelete("[action]")]
        public ActionResult Delete()
        {
            return Ok();
        }
    }
}
