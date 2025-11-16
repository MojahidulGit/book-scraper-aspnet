using BookScraperProject.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BookScraperProject.Services
{
    public class BookScraperService : IBookScraperService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookScraperService> _logger;

        public BookScraperService(HttpClient httpClient, ILogger<BookScraperService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<ScrapingResult> ScrapeBooksAsync(int maxBooks = 500)
        {
            var startTime = DateTime.Now;
            var books = new List<Book>();
            string baseUrl = "https://books.toscrape.com/";
            string currentUrl = baseUrl;
            int booksScraped = 0;

            try
            {
                while (booksScraped < maxBooks && currentUrl != null)
                {
                    _logger.LogInformation($"Scraping page: {currentUrl}");

                    // Polite delay
                    await Task.Delay(1500);

                    var response = await _httpClient.GetAsync(currentUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"Failed to fetch page: {response.StatusCode}");
                        break;
                    }

                    var htmlContent = await response.Content.ReadAsStringAsync();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlContent);

                    // Extract books from current page
                    var bookNodes = htmlDoc.DocumentNode.SelectNodes("//article[@class='product_pod']");
                    if (bookNodes == null) break;

                    foreach (var bookNode in bookNodes)
                    {
                        if (booksScraped >= maxBooks) break;

                        var book = ExtractBookData(bookNode, baseUrl);
                        if (book != null)
                        {
                            books.Add(book);
                            booksScraped++;
                        }
                    }

                    _logger.LogInformation($"Scraped {booksScraped} books so far...");

                    // Find next page
                    currentUrl = GetNextPageUrl(htmlDoc, baseUrl, currentUrl);
                }

                // Save to JSON file
                await SaveToJsonFile(books);

                return new ScrapingResult
                {
                    TotalBooksScraped = booksScraped,
                    Status = "Success",
                    Books = books,
                    TimeTaken = DateTime.Now - startTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scraping");
                return new ScrapingResult
                {
                    TotalBooksScraped = booksScraped,
                    Status = $"Error: {ex.Message}",
                    Books = books,
                    TimeTaken = DateTime.Now - startTime
                };
            }
        }

        public async Task<ScrapingResult> ScrapeAllBooksAsync()
        {
            return await ScrapeBooksAsync(1000);
        }

        private Book ExtractBookData(HtmlNode bookNode, string baseUrl)
        {
            try
            {
                var book = new Book();

                // Extract title
                book.Title = bookNode.SelectSingleNode(".//h3/a")?.GetAttributeValue("title", "")?.Trim() ?? "";

                // Extract price
                book.Price = bookNode.SelectSingleNode(".//p[@class='price_color']")?.InnerText?.Trim() ?? "";

                // Extract rating
                var ratingClass = bookNode.SelectSingleNode(".//p[contains(@class, 'star-rating')]")?
                    .GetAttributeValue("class", "") ?? "";
                book.Rating = ConvertRating(ratingClass);

                // Extract product link
                var relativeLink = bookNode.SelectSingleNode(".//h3/a")?.GetAttributeValue("href", "") ?? "";
                book.ProductLink = relativeLink != "" ? new Uri(new Uri(baseUrl), relativeLink).ToString() : "";

                // Extract image URL
                var relativeImage = bookNode.SelectSingleNode(".//img")?.GetAttributeValue("src", "") ?? "";
                book.ImageUrl = relativeImage != "" ? new Uri(new Uri(baseUrl), relativeImage).ToString() : "";

                // Stock availability
                book.StockAvailability = "In stock";

                return book;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error extracting book data: {ex.Message}");
                return null;
            }
        }

        private string ConvertRating(string ratingClass)
        {
            if (string.IsNullOrEmpty(ratingClass)) return "Not rated";

            var ratingMap = new Dictionary<string, string>
            {
                {"One", "1 star"},
                {"Two", "2 stars"},
                {"Three", "3 stars"},
                {"Four", "4 stars"},
                {"Five", "5 stars"}
            };

            foreach (var (key, value) in ratingMap)
            {
                if (ratingClass.Contains(key))
                    return value;
            }

            return "Not rated";
        }

        private string GetNextPageUrl(HtmlDocument htmlDoc, string baseUrl, string currentUrl)
        {
            var nextButton = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='next']/a");
            if (nextButton == null) return null;

            var relativeUrl = nextButton.GetAttributeValue("href", "");
            return new Uri(new Uri(currentUrl), relativeUrl).ToString();
        }

        private async Task SaveToJsonFile(List<Book> books)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(books,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });

            await File.WriteAllTextAsync("BooksData.json", json);
        }
    }
}