using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Author).Include(p=>p.Comments).Include(p=>p.Tags);
            return View(posts.ToList());
        }



        public ActionResult SearchTitles(string SearchString)
        {
            string SearchTitle = SearchString.ToLower().Trim();

            var posts = db.Posts.Include(p => p.Author).Include(p => p.Comments).Include(p => p.Tags);
            posts = posts.Where(p => p.Title.ToLower().Contains(SearchTitle));

            return View("Index",posts.ToList());
        }


        public ActionResult SearchTags(string SearchString)
        {
            string SearchTag = SearchString.ToLower().Trim();

            var posts = db.Posts.Include(p => p.Author).Include(p => p.Comments).Include(p => p.Tags);
            var posts2 = new List<Post>();

            foreach (var post in posts)
            {
                foreach (var tag in post.Tags)
                {
                    if (tag.Name== SearchTag)
                    {
                        posts2.Add(post);
                    }
                }
            }
            

            return View("Index", posts2);
        }

        public ActionResult SearchAuthors(string SearchString)
        {
            string SearchAuthor = SearchString.ToLower().Trim();

            var posts = db.Posts.Include(p => p.Author).Include(p => p.Comments).Include(p => p.Tags);
            posts = posts.Where(p => p.Author.UserName.ToLower().Contains(SearchAuthor));

            return View("Index", posts.ToList());
        }

        public ActionResult SearchPosts(string SearchString)
        {
            string SearchPost = SearchString.ToLower().Trim();

            var posts = db.Posts.Include(p => p.Author).Include(p => p.Comments).Include(p => p.Tags);
            posts = posts.Where(p => p.Body.ToLower().Contains(SearchPost));

            return View("Index", posts.ToList());
        }



        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var postView = db.Posts.SingleOrDefault(p=>p.Id==id);
            postView.ViewCount++;
            db.SaveChanges();

            Post post = db.Posts.Include(p => p.Author).Include(p=>p.Comments).SingleOrDefault(p => p.Id == id);
            var comments = db.Comments.Where(c => c.Post.Id == id).Include(c => c.Author);
            ViewBag.Comments = comments;
           
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string TagString,[Bind(Include = "Id,Title,Body")] Post post)
        {
            if (ModelState.IsValid)
            {
                //creating new tags in the tag table

                var tags = TagString
                    .Split(',')
                    .Select(t => t.ToLower())
                    .Select(t => t.Trim())
                    .Distinct();

                foreach (var name in tags)
                {
                    //tags are unique->checking if tag already exists

                    var tagCheck = db.Tags.FirstOrDefault(t => t.Name == name);
                    if (!db.Tags.ToList().Contains(tagCheck))
                    {
                        var tag = new Tag();
                        tag.Name =name;
                        db.Tags.Add(tag);
                    }

                }
               
                //creating the post

                post.Date = DateTime.Now;
                post.Author = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                db.Posts.Add(post);
                db.SaveChanges();

                //adding the new tags to the post and updating the table
                foreach (var name in tags)
                {
                    var tag = db.Tags.FirstOrDefault(t => t.Name == name);
                    db.Posts.Include("Tags").FirstOrDefault(p => p.Id == post.Id).Tags.Add(tag);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Include(p => p.Author).SingleOrDefault(p => p.Id == id);

            if (post == null || post.Author.UserName!=User.Identity.Name && !User.IsInRole("Administrator"))
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Date")] Post post)
        {
            
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Include(p => p.Author).SingleOrDefault(p => p.Id == id);

            if (post == null || post.Author.UserName != User.Identity.Name && !User.IsInRole("Administrator"))
            {
                return HttpNotFound();
            }

            
            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

       
    }
}
