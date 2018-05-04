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
		public ActionResult StaffArea(ItemLibrarianViewModel libObj, int numCopies)
		{
			using (LibraryEntities db = new LibraryEntities())
			{
				// Add books.
				if (ModelState.IsValid)
				{

					Item it = new Item();
					it.AuthorID = libObj.Aut.AuthorID;
					it.Name = libObj.It.Name;
					it.Isbn = libObj.It.Isbn;
					it.Subject = libObj.It.Subject;
					it.Type = "Book";
					it.Year = libObj.It.Year;
					libObj.Aut.Isbn = libObj.It.Isbn;
					db.Authors.Add(libObj.Aut);
					db.Items.Add(it);
					for (int i = 0; i < numCopies; i++)
					{
						Copy cp = new Copy();
						cp.Isbn = libObj.It.Isbn;
						cp.Borrowed = "n";
						db.Copies.Add(cp);
					}
					db.SaveChanges();
					if (libObj.It.Isbn > 0)
					{
						ViewBag.Success = libObj.It.Name.ToString();

					}
					ModelState.Clear();
				}
				//Add students
				if (ModelState.IsValid)
				{
					Customer cust = new Customer();
					cust.CustName = libObj.C.CustName;
					cust.CustEmail = libObj.C.CustEmail;
					cust.Field = libObj.C.Field;
					cust.Privalige = libObj.C.Privalige;
					cust.CPassword = libObj.C.CPassword;
					db.Customers.Add(cust);
					db.SaveChanges();
					ModelState.Clear();
				}

				return View();
			}//end ModelStateif
		}//end ActionResult 
		public ActionResult LogOut()
		{
			TempData["logOutMessage"] = "Successfully Logged Out";
			return RedirectToAction("StaffLogin");
		}

		public ActionResult SubjectGuide()
		{
			using (LibraryEntities db = new LibraryEntities())
			{
				var books = from Aut in db.Authors
							join It in db.Items
							on Aut.AuthorID equals It.AuthorID
							select new ItemLibrarianViewModel { It = It, Aut = Aut };

				books.ToList();
				return View(books);
			}

				
		}
	}
}

