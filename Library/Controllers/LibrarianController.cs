using Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
	public class LibrarianController : Controller
	{
		// GET: Librarian
		public ActionResult StaffLogin()
		{
			if (TempData["logOutMessage"] != null)
			{
				ViewBag.LogOutMessage = TempData["logOutMessage"].ToString();
			}

			return View();

		}

		//Tells runtime where to send form post data back to
		[HttpPost]
		//Protects against hackers
		[ValidateAntiForgeryToken]
		public ActionResult StaffLogin(Librarian libUser)
		{
			//Check that form data is in correct format for the model and obeys all the rules
			if (ModelState.IsValid)
			{
				//Database object for accessing data
				using (LibraryEntities db = new LibraryEntities())
				{
					//Check the password and ID match
					var obj = db.Librarians.Where(a => a.LibrarianID.Equals(libUser.LibrarianID) && a.Password.Equals(libUser.Password)).FirstOrDefault();
					Debug.WriteLine(obj.Name.ToString());
					if (obj != null)
					{
						//Set the session variables with values from the database
						Session["libID"] = obj.LibrarianID.ToString();
						Session["libLocation"] = obj.LibLocation.ToString();
						Session["libName"] = obj.Name.ToString();
						Session["pos"] = obj.Position.ToString();
						//redirect to the librarian page
						return RedirectToAction("StaffArea");

					}

				}
			}
			return View(libUser);

		}

		public ActionResult StaffArea()
		{
			if (Session["libID"] != null)
			{

				return View();
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult StaffArea(ItemLibrarianViewModel bookObj, int numCopies, Customer custObj)
		{
			using (LibraryEntities db = new LibraryEntities())
			{
				// Add books.
				if (ModelState.IsValid)
				{

					Item it = new Item();
					it.AuthorID = bookObj.Aut.AuthorID;
					it.Name = bookObj.It.Name;
					it.Isbn = bookObj.It.Isbn;
					it.Subject = bookObj.It.Subject;
					it.Type = "Book";
					it.Year = bookObj.It.Year;
					bookObj.Aut.Isbn = bookObj.It.Isbn;
					db.Authors.Add(bookObj.Aut);
					db.Items.Add(it);
					for (int i = 0; i < numCopies; i++)
					{
						Copy cp = new Copy();
						cp.Isbn = bookObj.It.Isbn;
						cp.Borrowed = "n";
						db.Copies.Add(cp);
					}
					db.SaveChanges();
					if (bookObj.It.Isbn > 0)
					{
						ViewBag.Success = bookObj.It.Name.ToString();

					}
					ModelState.Clear();
				}
				//Add students
				if (ModelState.IsValid)
				{
					Customer cust = new Customer();
					cust.CustName = custObj.CustName;
					cust.CustEmail = custObj.CustEmail;
					cust.Field = custObj.Field;
					cust.Privalige = custObj.Privalige;
					cust.CPassword = custObj.CPassword;
				
				}

				return View();
			}
		}
		public ActionResult LogOut()
		{
			TempData["logOutMessage"] = "Successfully Logged Out";
			return RedirectToAction("StaffLogin");
		}
	}
}

