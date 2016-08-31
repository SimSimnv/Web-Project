using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Author)
                .Include(p=>p.Tags)
                .Include(p=>p.Comments)
                .Where(p=>p.IsAnnouncement==true)
                .OrderByDescending(p => p.Date).Take(4).ToList();

            
            
            // Sidebar
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Post).OrderByDescending(c => c.Date).Take(5);
            ViewBag.Comments = comments;

            var PopularPosts = db.Posts.OrderByDescending(p => p.ViewCount).Take(5);
            ViewBag.PopularPosts = PopularPosts;

            var NewestPosts = db.Posts.OrderByDescending(p => p.Date).Take(5);
            ViewBag.NewestPosts = NewestPosts;
            
            return View(posts);
        }

        public ActionResult UserDetails(string UserName)
        {
            var posts = db.Posts.Include(p => p.Author).Include(p => p.Comments).Include(p => p.Tags).Where(p => p.Author.UserName == UserName);
            ViewBag.Author = posts.First().Author;

            var comments = db.Comments.Include(c=>c.Post).Where(c => c.Author.UserName == posts.FirstOrDefault().Author.UserName).ToList();

            ViewBag.Comments = comments;
           
            return View(posts.ToList());
        }

        [Authorize(Roles ="Administrator")]
        public ActionResult AdminPanel()
        {

            var users = db.Users.ToList();
            return View(users);

        }
        [Authorize(Roles = "Administrator")]
        public ActionResult AddAdmin(string id)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            userManager.AddToRole(id, "Administrator");
            db.SaveChanges();

            var users = db.Users.ToList();

            return View("AdminPanel",users);

        }
    }
}