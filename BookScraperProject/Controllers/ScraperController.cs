using BookScraperProject.Models;
using BookScraperProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookScraperProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IBookScraperService _scraperService;
        private readonly ILogger<ScraperController> _logger;

        public ScraperController(IBookScraperService scraperService, ILogger<ScraperController> logger)
        {
            _scraperService = scraperService;
            _logger = logger;
        }

        [HttpPost("scrape")]
        public async Task<IActionResult> ScrapeBooks([FromQuery] int maxBooks = 500)
        {
            try
            {
                _logger.LogInformation($"Starting to scrape {maxBooks} books");
                var result = await _scraperService.ScrapeBooksAsync(maxBooks);

                return Ok(new
                {
                    Status = "Completed",
                    TotalBooks = result.TotalBooksScraped,
                    TimeTaken = result.TimeTaken.ToString(),
                    Message = $"Successfully scraped {result.TotalBooksScraped} books"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in scraping controller");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("scrape-all")]
        public async Task<IActionResult> ScrapeAllBooks()
        {
            try
            {
                _logger.LogInformation("Starting to scrape all books");
                var result = await _scraperService.ScrapeAllBooksAsync();

                return Ok(new
                {
                    Status = "Completed",
                    TotalBooks = result.TotalBooksScraped,
                    TimeTaken = result.TimeTaken.ToString(),
                    Message = $"Successfully scraped {result.TotalBooksScraped} books"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in scraping controller");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("download")]
        public IActionResult DownloadData()
        {
            if (!System.IO.File.Exists("BooksData.json"))
            {
                return NotFound("No data file found. Please run the scraper first.");
            }

            var bytes = System.IO.File.ReadAllBytes("BooksData.json");
            return File(bytes, "application/json", "BooksData.json");
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var fileExists = System.IO.File.Exists("BooksData.json");
            int bookCount = 0;

            if (fileExists)
            {
                try
                {
                    var json = System.IO.File.ReadAllText("BooksData.json");
                    var books = System.Text.Json.JsonSerializer.Deserialize<List<Book>>(json);
                    bookCount = books?.Count ?? 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading data file");
                }
            }

            return Ok(new
            {
                DataFileExists = fileExists,
                TotalBooksInFile = bookCount,
                LastModified = fileExists ? System.IO.File.GetLastWriteTime("BooksData.json") : (DateTime?)null
            });
        }
    }
}