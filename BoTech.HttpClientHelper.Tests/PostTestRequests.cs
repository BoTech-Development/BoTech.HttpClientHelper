namespace BoTech.HttpClientHelper.Tests;

[TestClass]
public class PostTestRequests
{
    private static HttpRequestHelper _httpHelper;
    private static TestDto _standardJsonReturn;
    private static TestDto _standardJsonRequest;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        //Please start the server before running the tests
        _httpHelper = new HttpRequestHelper("https://localhost:7188/");
        _standardJsonRequest = new TestDto()
        {
            Name = "Florian",
            Age = 19,
            Birthday = new DateTime(2030, 1, 1)
        };
        _standardJsonReturn = new TestDto()
        {
            Name = "FloBo",
            Age = 19,
            Birthday = new DateTime(2030, 1, 1)
        };
    }

    [TestMethod]
    public void TestPostJsonAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpPostJsonAndGetJson<TestDto>("/api/Test/PostJsonAndGetJson", _standardJsonRequest).Result;
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }
    [TestMethod]
    public void TestPostHttpContentAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpPostContentAndGetJson<TestDto>("/api/Test/PostHttpContentAndGetJson", new StringContent("FloBo")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }

    [TestMethod]
    public void TestPostJson()
    {
        RequestResult<dynamic> result = _httpHelper.HttpPostJson("/api/Test/PostJson", _standardJsonRequest).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
    
    [TestMethod]
    public void TestPost()
    {
        RequestResult<dynamic> result = _httpHelper.HttpPost("/api/Test/Post", new StringContent("")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
}