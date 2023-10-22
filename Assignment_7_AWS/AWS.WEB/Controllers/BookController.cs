using AWS.Application;
using AWS.Domain;
using AWS.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AWS.WEB.Controllers
{
    public class BookController : Controller
    {
        private readonly IDynamoDbService _dynamoDbService;

        public BookController(IDynamoDbService dynamoDbService)
        {
            _dynamoDbService = dynamoDbService;
        }

        public ActionResult CreateBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddData(Book book)
        {
            try
            {
                await _dynamoDbService.AddBookAsync(book);
                return RedirectToAction("BookList"); 
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to add book: " + ex.Message);
            }
        }

        public IActionResult BookList()
        {
            return View();
        }
        
        public async Task<IActionResult> LoadBookData()
        {
            var records = await _dynamoDbService.GetBookListAsync();
            var books = new List<AWS.Domain.Book>();

            foreach (var record in records)
            {
                if (record.TryGetValue("BookId", out var bookIdAttribute) && int.TryParse(bookIdAttribute.N, out var bookId))
                {
                    var book = new AWS.Domain.Book
                    {
                        BookId = bookId,
                        Title = record.TryGetValue("Title", out var titleAttribute) ? titleAttribute.S : null,
                        Author = record.TryGetValue("Author", out var authorAttribute) ? authorAttribute.S : null,
                    };
                    books.Add(book);
                }
            }
            var data = books.Select(b => new
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
            });

            return Json(new
            {
                draw = 1,
                recordsTotal = data.Count(),
                recordsFiltered = data.Count(),
                data = data
            });
        }
        public async Task<IActionResult> EditBook(string id)
        {
            Book book = await _dynamoDbService.GetBookByIdAsync(id);

            if (book == null)
            {                return RedirectToAction("BookList"); 
            }

            // Pass the book data to the view for editing
            return View(book);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Book model)
        {
            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    BookId = model.BookId,
                    Title = model.Title,
                    Author = model.Author
                   
                };
                await _dynamoDbService.EditBookAsync(book);
            }
            return RedirectToAction("BookList"); 
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _dynamoDbService.DeleteBookAsync(id);
                return RedirectToAction("BookList"); 
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to delete book: " + ex.Message);
            }
        }
    }
}
