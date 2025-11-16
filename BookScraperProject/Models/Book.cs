using System;
using System.Collections.Generic;

namespace BookScraperProject.Models
{
    public class Book
    {
        public string Title { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public string StockAvailability { get; set; } = string.Empty;
        public string ProductLink { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class ScrapingResult
    {
        public int TotalBooksScraped { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new List<Book>();
        public TimeSpan TimeTaken { get; set; }
    }
}