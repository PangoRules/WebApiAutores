using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApiAutores.Controllers;
using WebApiAutores.Entities;

namespace WebApiAutores.UnitTests
{
    public class AuthorsControllerUnitTests
    {
        //private Mock<ApplicationDbContext> _mockContext;
        //private DbContextOptionsBuilder _optionsBuilder;

        //private Author _author;
        //private AuthorsController _mockController;

        //[SetUp]
        //public void Setup()
        //{
        //    _author = new Author
        //    {
        //        Id = It.IsAny<int>(),
        //        Name = It.IsAny<string>(),
        //    };

        //    _optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        //    _mockContext = new Mock<ApplicationDbContext>();

        //    //_mockController = new AuthorsController(_mockContext.Object);
        //}

        //[Test]
        //public void Get_WhenCalled_ReturnListOfAuthors()
        //{
        //    var result = _mockController.Get();

        //    Assert.That(result, Is.TypeOf<ActionResult>());
        //}
    }
}