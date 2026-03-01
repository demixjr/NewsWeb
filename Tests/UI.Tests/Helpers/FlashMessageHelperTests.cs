using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PL.Helpers;
using Xunit;

namespace UI.Tests
{
    public class FlashMessageHelperTests
    {
        private readonly ITempDataDictionary _tempData;

        public FlashMessageHelperTests()
        {
            var mock = new Mock<ITempDataDictionary>();
            var store = new Dictionary<string, object?>();
            mock.Setup(t => t[It.IsAny<string>()])
                .Returns((string key) => store.GetValueOrDefault(key));
            mock.SetupSet(t => t[It.IsAny<string>()] = It.IsAny<object?>())
                .Callback((string key, object? value) => store[key] = value);
            _tempData = mock.Object;
        }

        [Fact]
        public void SetSuccess_StoresMessage()
        {
            FlashMessageHelper.SetSuccess(_tempData, "Saved!");
            Assert.Equal("Saved!", _tempData["Success"]);
        }

        [Fact]
        public void SetError_WithString_StoresMessage()
        {
            FlashMessageHelper.SetError(_tempData, "Error occurred");
            Assert.Equal("Error occurred", _tempData["Error"]);
        }

        [Fact]
        public void SetError_WithException_StoresExceptionMessage()
        {
            var ex = new Exception("Something went wrong");
            FlashMessageHelper.SetError(_tempData, ex);
            Assert.Equal("Something went wrong", _tempData["Error"]);
        }
    }
}