using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class BookController : Controller
    {
		// GET: Book
		public ActionResult BookList()
		{
			LibraryEntities db = new LibraryEntities();
			var books = from Aut in db.Authors
						join It in db.Items
						on Aut.AuthorID equals It.AuthorID
						select new ItemLibrarianViewModel { It = It, Aut = Aut};

			books.ToList();
			/*var books = from b in db.Items
						join a in db.Authors
						on b.AuthorID equals a.AuthorID
						select new { a.AuthName, b.Isbn, b.Name, b.Subject, b.Year };
			books.ToList();
			ViewData["booklist"] = books;*/
			

			return View(books);
        }
    }
}