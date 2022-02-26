using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using order_book_backend.Controllers;
using order_book_backend.Model;
using order_book_backend.Services;
using System.Threading.Tasks;

namespace order_book_backend.UnitTests
{
    [TestClass]
    public class OrderBookControllerTests
    {
        [TestMethod]
        public async Task Get_ReturnsNoContent()
        {
            // Arrange
            IOrderBookService orderBookServiceMock = Mock.Of<IOrderBookService>();
            ILogger<OrderBookController> loggerMock = Mock.Of<ILogger<OrderBookController>>();
            OrderBookController controller = new(loggerMock, orderBookServiceMock);
            OrderBookRequest request = new()
            {
                CurrencyPair = "btceur"
            };

            // Act
            IActionResult result = await controller.Get(request); // No result because a mock will not retrieve actual result
            NoContentResult? noContentResult = result as NoContentResult;

            // Assert
            Assert.IsTrue(result is NoContentResult);
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest()
        {
            // Arrange
            IOrderBookService orderBookServiceMock = Mock.Of<IOrderBookService>();
            ILogger<OrderBookController> loggerMock = Mock.Of<ILogger<OrderBookController>>();
            OrderBookController controller = new(loggerMock, orderBookServiceMock);
            OrderBookRequest request = new()
            {
                CurrencyPair = "eur" // Unsupported currency pair
            };

            // Act
            IActionResult result = await controller.Get(request);
            ObjectResult? badRequestResult = result as ObjectResult;

            // Assert
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }
}
