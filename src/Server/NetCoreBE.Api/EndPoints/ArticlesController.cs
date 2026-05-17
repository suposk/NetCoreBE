using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using Xunit.Abstractions;
//using Xunit.Sdk;

namespace dotnet_articles_api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private IRepository _repository;

        private readonly LoggerProxy _logger;

        public ArticlesController(IRepository repository, LoggerProxy logger)
        {
            _logger = logger; // use _logger.WriteLine() to write to the console.
            _repository = repository;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            // _logger.WriteLine("Sample debug output");
            return new BadRequestResult();
        }
    }
}

public interface IRepository
{
    // Returns a found article or null.
    Article Get(Guid id);
    // Creates a new article and returns its identifier.
    // Throws an exception if a article is null.
    // Throws an exception if a title is null or empty.
    Guid Create(Article article);
    // Returns true if an article was deleted or false if it was not possible to find it.
    bool Delete(Guid id);
    // Returns true if an article was updated or false if it was not possible to find it.
    // Throws an exception if an articleToUpdate is null.
    // Throws an exception or if a title is null or empty.
    bool Update(Article articleToUpdate);
}



public class Article
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
}

// Add this class definition to resolve CS0246 for LoggerProxy
public class LoggerProxy
{
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }
}

