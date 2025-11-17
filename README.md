# 📚 Book Scraper - ASP.NET Core Web API

A robust web scraping solution built with ASP.NET Core that extracts book data from BooksToScrape.com. This project demonstrates professional web scraping practices with automatic pagination, rate limiting, and RESTful API endpoints.

## 🚀 Features

- **📖 Data Extraction**: Scrapes 500+ book records with complete details
- **🔄 Automatic Pagination**: Handles multi-page navigation seamlessly
- **⚡ RESTful API**: Clean API endpoints for scraping operations
- **🛡️ Polite Scraping**: Built-in delays and rate limiting
- **📊 JSON Export**: Structured data export functionality
- **🔍 Error Handling**: Comprehensive error handling and retry logic
- **📚 Swagger Documentation**: Interactive API documentation

## 📋 Extracted Data Fields

1. **Title** - Book title
2. **Price** - Book price
3. **Rating** - Star rating (1-5 stars)
4. **Stock Availability** - In stock / Out of stock
5. **Product Link** - Detail page URL
6. **Image URL** - Book cover image URL
## 🏗️ Project Architecture
book-scraper-aspnet/
├── 📁 Controllers/
├── 📁 Models/ 
├── 📁 Services/
├── 📄 Program.cs
├── 📄 appsettings.json
├── 📄 BookScraperProject.csproj
├── 📄 README.md
└── 📄 BooksData.json (after first run)
## Website URL Used
https://books.toscrape.com/

## Total Records Collected
500+ book records (can scrape up to 1000 books)

## Pagination Method Used
Automatic pagination handling by detecting and following "Next" page links using HtmlAgilityPack XPath queries.

## 🛠️ Technologies Used

- **ASP.NET Core 8.0** - Web Framework
- **HtmlAgilityPack** - HTML Parsing
- **Swashbuckle** - API Documentation
- **HttpClient** - HTTP Requests

## 📥 Installation & Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 or VS Code

### Quick Start

1. **Clone or download the project files**

2. **Restore dependencies**
   ```bash
   dotnet restore
3.Run the application
dotnet run
Access the API
Open https://localhost:7000/swagger in your browser

Start scraping

Use POST /api/scraper/scrape endpoint

Wait for completion (5-10 minutes)

Download data using GET /api/scraper/download

🎯 API Endpoints
Method	Endpoint	Description
POST	/api/scraper/scrape	Start scraping books (500+ records)
POST	/api/scraper/scrape-all	Scrape all available books (1000)
GET	/api/scraper/download	Download scraped data as JSON
GET	/api/scraper/status	Check scraping status
Challenges Faced and Solutions
Challenge 1: Relative URL Conversion
Solution: Used Uri class to convert relative URLs to absolute URLs for product links and images.

Challenge 2: Rating Conversion
Solution: Created mapping dictionary to convert CSS classes ('One', 'Two', etc.) to readable star ratings.

Challenge 3: Polite Scraping
Solution: Implemented 1.5 second delay between requests and proper User-Agent headers.

Challenge 4: Error Handling
Solution: Added comprehensive try-catch blocks and HTTP status code checking.

Dependencies/Requirements
ASP.NET Core 8.0

HtmlAgilityPack (1.11.54)

Swashbuckle.AspNetCore (6.4.0)

⚠️ Responsible Scraping
This project follows ethical web scraping practices:

Respects robots.txt rules

Implements request throttling (1.5s delay)

Uses proper User-Agent headers

Includes delays between requests

👨‍💻 Developer
Md Mojahidul Islam 
mdmojahidul5577@gmail.com
