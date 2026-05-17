using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using Xunit.Abstractions;
//using Xunit.Sdk;


/*
  
using System.Collections.Generic;
using System.Linq;
 */

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
            try
            {
                // Validate that the GUID is not empty
                if (id == Guid.Empty)
                {
                    _logger.WriteLine("Invalid GUID: empty GUID provided");
                    return BadRequest("Invalid article ID. GUID cannot be empty.");
                }

                // Retrieve article from repository
                var article = _repository.Get(id);

                // Check if article was found
                if (article == null)
                {
                    _logger.WriteLine($"Article not found for ID: {id}");
                    return NotFound($"Article with ID '{id}' not found.");
                }

                _logger.WriteLine($"Article retrieved successfully: {article.Title}");
                return Ok(article);
            }
            catch (ArgumentNullException ex)
            {
                _logger.WriteLine($"ArgumentNullException in Get method: {ex.Message}");
                return BadRequest("Invalid request parameters.");
            }
            catch (ArgumentException ex)
            {
                _logger.WriteLine($"ArgumentException in Get method: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"Unexpected error in Get method: {ex.GetType().Name} - {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while retrieving the article.");
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Article article)
        {
            try
            {
                // Validate that the request body is not null
                if (article == null)
                {
                    _logger.WriteLine("Create request received with null article body");
                    return StatusCode(400, "Article cannot be null.");
                }

                // Validate model state (handles data annotations validation)
                if (!ModelState.IsValid)
                {
                    _logger.WriteLine($"Invalid model state: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)))}");
                    return StatusCode(400, ModelState);
                }

                // Validate Title
                if (string.IsNullOrWhiteSpace(article.Title))
                {
                    _logger.WriteLine("Create request received with null or empty title");
                    return StatusCode(400, "Title is required and cannot be empty.");
                }

                // Create the article
                var createdId = _repository.Create(article);

                _logger.WriteLine($"Article created successfully with ID: {createdId}, Title: {article.Title}");

                // Return 201 with Location header
                Response.Headers.Location = $"/api/articles/{createdId}";
                return StatusCode(201, article);
            }
            catch (ArgumentNullException ex)
            {
                _logger.WriteLine($"ArgumentNullException in Create method: {ex.Message}");
                return StatusCode(400, "Invalid article data.");
            }
            catch (ArgumentException ex)
            {
                _logger.WriteLine($"ArgumentException in Create method: {ex.Message}");
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"Unexpected error in Create method: {ex.GetType().Name} - {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while creating the article.");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                // Validate that the GUID is not empty
                if (id == Guid.Empty)
                {
                    _logger.WriteLine("Invalid GUID: empty GUID provided in Delete request");
                    return BadRequest("Invalid article ID. GUID cannot be empty.");
                }

                // Delete the article from repository
                var isDeleted = _repository.Delete(id);

                // Check if article was deleted
                if (!isDeleted)
                {
                    _logger.WriteLine($"Article not found for deletion with ID: {id}");
                    ///return NotFound($"Article with ID '{id}' not found.");
                    return NotFound();
                }

                _logger.WriteLine($"Article deleted successfully with ID: {id}");
                return Ok();
            }
            catch (ArgumentNullException ex)
            {
                _logger.WriteLine($"ArgumentNullException in Delete method: {ex.Message}");
                return BadRequest("Invalid request parameters.");
            }
            catch (ArgumentException ex)
            {
                _logger.WriteLine($"ArgumentException in Delete method: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"Unexpected error in Delete method: {ex.GetType().Name} - {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while deleting the article.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] Article article)
        {
            try
            {
                // Validate that the GUID is not empty
                if (id == Guid.Empty)
                {
                    _logger.WriteLine("Invalid GUID: empty GUID provided in Update request");
                    return BadRequest("Invalid article ID. GUID cannot be empty.");
                }

                // Validate that the request body is not null
                if (article == null)
                {
                    _logger.WriteLine("Update request received with null article body");
                    return StatusCode(404, "Article cannot be null.");
                }

                // Validate model state (handles data annotations validation)
                if (!ModelState.IsValid)
                {
                    _logger.WriteLine($"Invalid model state: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)))}");
                    return StatusCode(404, ModelState);
                }

                // Validate Title
                if (string.IsNullOrWhiteSpace(article.Title))
                {
                    _logger.WriteLine("Update request received with null or empty title");
                    return BadRequest("Title is required and cannot be empty.");
                }

                // Validate Text
                if (string.IsNullOrWhiteSpace(article.Text))
                {
                    _logger.WriteLine("Update request received with null or empty text");
                    return StatusCode(405, "Text is required and cannot be empty.");
                }

                // Set the article ID from the URL
                article.Id = id;

                // Update the article in repository
                var isUpdated = _repository.Update(article);

                // Check if article was updated
                if (!isUpdated)
                {
                    _logger.WriteLine($"Article not found for update with ID: {id}");
                    return NotFound($"Article with ID '{id}' not found.");
                }

                _logger.WriteLine($"Article updated successfully with ID: {id}, Title: {article.Title}");
                return Ok();
            }
            catch (ArgumentNullException ex)
            {
                _logger.WriteLine($"ArgumentNullException in Update method: {ex.Message}");
                return BadRequest("Invalid article data.");
            }
            catch (ArgumentException ex)
            {
                _logger.WriteLine($"ArgumentException in Update method: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"Unexpected error in Update method: {ex.GetType().Name} - {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while updating the article.");
            }
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
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(200)]
    public string? Title { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string? Text { get; set; }
}

// Add this class definition to resolve CS0246 for LoggerProxy
public class LoggerProxy
{
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    //public void Error(string message)
    //{
    //    Console.Error.WriteLine($"ERROR: {message}");
    //}
}

// In your Program.cs or Startup configuration


