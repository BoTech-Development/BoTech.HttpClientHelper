using BoTech.HttpClientHelper.Models;

namespace BoTech.HttpClientHelper.Tests
{
    [TestClass]
    public sealed class GetTestRequests
    {
        private static HttpRequestHelper _httpHelper;
        private static TestDto _standardJsonReturn;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            //Please start the server before running the tests
            _httpHelper = new HttpRequestHelper("https://localhost:7188/");
            _standardJsonReturn = new TestDto()
            {
                Name = "Florian",
                Age = 19,
                Birthday = new DateTime(2030, 1, 1)
            };
        }

        [TestMethod]
        public void TestGetAndStoreFile()
        {
            RequestResult result =_httpHelper.HttpGetFileAndCopyTo("TestFile.txt", "someFile.txt").Result;
            if(!result.IsSuccess())
                Assert.Fail(result.Error.Message);  
        }
        [TestMethod]
        public void TestGetContentsOfFile()
        {
            RequestResult result =_httpHelper.HttpGetFileContents("TestFile.txt").Result;
            if(!result.IsSuccess())
                Assert.Fail(result.Error.Message);
            if (result.ParsedData!.ToString()!.Equals("This is a test file.") == false) 
                Assert.Fail("File contents do not match");
        }
        [TestMethod]
        public void TestGetJson()
        {
            RequestResult result =_httpHelper.HttpGetJsonObject("/api/Test/GetJson", JsonDtoSelectionOptions.CreateForSingleStatusOkDtoType(typeof(TestDto))).Result;
            if(!result.IsSuccess())
                Assert.Fail(result.Error.Message);
            if(!result.ParsedData.Equals(_standardJsonReturn))
                Assert.Fail("Json data does not match");
        }
        [TestMethod]
        public void TestGetString()
        {
            RequestResult result =_httpHelper.HttpGetString("/api/Test/GetString").Result;
            if(!result.IsSuccess())
                Assert.Fail(result.Error.Message);
            if(result.ParsedData!.ToString()!.Equals("This is a test string") == false)
                Assert.Fail("String data does not match");
        }
    }
}
