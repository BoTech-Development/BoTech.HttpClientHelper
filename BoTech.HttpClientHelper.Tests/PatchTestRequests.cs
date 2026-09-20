namespace BoTech.HttpClientHelper.Tests;

[TestClass]
public class PatchTestRequests
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
    public void TestPatchPatchJsonAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpPatchJsonAndGetJson<TestDto>("/api/Test/PatchJsonAndGetJson", _standardJsonRequest).Result;
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }
    [TestMethod]
    public void TestPatchHttpContentAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpPatchContentAndGetJson<TestDto>("/api/Test/PatchHttpContentAndGetJson", new StringContent("FloBo")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }

    [TestMethod]
    public void TestPatchJson()
    {
        RequestResult<dynamic> result = _httpHelper.HttpPatchJson("/api/Test/PatchJson", _standardJsonRequest).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
    
    [TestMethod]
    public void TestPatch()
    {
        RequestResult<dynamic> result = _httpHelper.HttpPatch("/api/Test/Patch", new StringContent("")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
}