namespace BoTech.HttpClientHelper.Tests;

[TestClass]
public class DeleteTestRequests
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
    public void TestDeleteJsonAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpDeleteJsonAndGetJson<TestDto>("/api/Test/DeleteJsonAndGetJson", _standardJsonRequest).Result;
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }
    [TestMethod]
    public void TestDeleteHttpContentAndGetJson()
    {
        RequestResult<TestDto> result = _httpHelper.HttpDeleteContentAndGetJson<TestDto>("/api/Test/DeleteHttpContentAndGetJson", new StringContent("FloBo")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
        if(!result.ParsedData.Equals(_standardJsonReturn))
            Assert.Fail("Json data does not match");
    }

    [TestMethod]
    public void TestDeleteJson()
    {
        RequestResult<dynamic> result = _httpHelper.HttpDeleteJson("/api/Test/DeleteJson", _standardJsonRequest).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
    
    [TestMethod]
    public void TestDelete()
    {
        RequestResult<dynamic> result = _httpHelper.HttpDelete("/api/Test/Delete", new StringContent("")).Result; // inserts into the Name property of the TestDto
        if(!result.IsSuccess())
            Assert.Fail(result.Error.Message);  
    }
}