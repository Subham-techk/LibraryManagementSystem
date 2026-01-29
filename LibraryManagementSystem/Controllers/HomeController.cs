using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Books.ToList());
        }
        public IActionResult IssueTo()
        {
            var data = _context.IssueBooks.Include(x => x.Book).ToList();

            return View(data);
        }
        [HttpGet]
        public IActionResult Search(string term)
        {
            var data = _context.Books
                .Where(x => x.Title.Contains(term))
                .Select(x => new {
                    x.Id,
                    x.Title,
                    x.Author,
                    x.AvailableCopies,
                    Issued = x.TotalCopies - x.AvailableCopies
                }).ToList();

            return Json(data);
        }

        [HttpPost]
        public IActionResult AddBook(Book book)
        {
            try
            {
                book.AvailableCopies = book.TotalCopies;
                _context.Books.Add(book);
                _context.SaveChanges();
                return Json(true);
            }
            catch (Exception ex) 
            { 
                return Json(ex);
            }
        }
        [HttpPost]
        public IActionResult EditBook(Book book)
        {
            try
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        [HttpPost]
        public IActionResult IssueBook(int bookId, string issuedTo)
        {
            try
            {
                var book = _context.Books.Find(bookId);
                if (book.AvailableCopies > 0)
                {
                    book.AvailableCopies--;
                    _context.IssueBooks.Add(new IssueBook
                    {
                        BookId = bookId,
                        IssuedTo = issuedTo,
                        IssueDate = DateTime.Now
                    });
                    _context.SaveChanges();
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                var book = _context.Books.Find(id);
                _context.Books.Remove(book);
                _context.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        [HttpPost]
        public IActionResult ReturnBook(int id)
        {
            try
            {
                var issueBook = _context.IssueBooks.Include(x => x.Book).FirstOrDefault(x => x.Id == id);

                if (issueBook == null)
                    return Json(false);
                if (issueBook.IsReturned)
                    return Json("Already Returned");
                issueBook.IsReturned = true;
                issueBook.ReturnDate = DateTime.Now;
                issueBook.Book.AvailableCopies += 1;
                _context.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
