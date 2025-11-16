using BookScraperProject.Models;
using System.Threading.Tasks;

namespace BookScraperProject.Services
{
    public interface IBookScraperService
    {
        Task<ScrapingResult> ScrapeBooksAsync(int maxBooks = 500);
        Task<ScrapingResult> ScrapeAllBooksAsync();
    }
}