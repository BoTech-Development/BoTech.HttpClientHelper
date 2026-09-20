namespace BoTech.HttpClientHelper.Tests;

[TestClass]
public class PutTestRequests
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
    public void TestPutJsonAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpPutJsonAndGetJson<TestDto>("/api/Test/PutJsonAndGetJson", _standardJsonRequest).Result;
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }
    [TestMethod]
    public void TestPutHttpContentAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpPutContentAndGetJson<TestDto>("/api/Test/PutHttpContentAndGetJson", new StringContent("FloBo")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }

    [TestMethod]
    public void TestPutJson()
    {
        RequestResult<dynamic> result = _httpHelper.HttpPutJson("/api/Test/PutJson", _standardJsonRequest).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
    
    [TestMethod]
    public void TestPut()
    {
        RequestResult<dynamic> result = _httpHelper.HttpPut("/api/Test/Put", new StringContent("")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
}