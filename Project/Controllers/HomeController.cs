using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using System.Data.Entity;

namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Author).OrderByDescending(p => p.Date).Take(4).ToList();
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Post).OrderByDescending(c => c.Date).Take(5);
            ViewBag.Comments = comments;
            
            return View(posts);
        }
    }
}