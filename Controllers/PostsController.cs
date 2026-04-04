using Microsoft.AspNetCore.Authorization;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {

        private readonly AppDbContext _db; // access to DB everywhere in the controller

        public PostsController(AppDbContext db)
        {
            _db = db;
        }

        //public - anybody can read
        public IActionResult Index()
        {
            var posts = _db.Posts.ToList(); //list all posts objects
            return View(posts); //to return posts page (view) automatically
        }
        public IActionResult Details(int id)
        {
            var post = _db.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET /Posts/Create — shows the empty form
        // Protected - must be logged in
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST /Posts/Create — receives and saves the form data
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now;
                _db.Posts.Add(post);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post); // validation failed — re-show form with errors
        }

        [Authorize]
        public IActionResult Edit(int id, Post post)
        {
            if (ModelState.IsValid)
            {
                var existing = _db.Posts.FirstOrDefault(p => p.Id == id);

                if (existing == null)
                {
                    return NotFound();
                }
                existing.Title = post.Title;
                existing.Content = post.Content;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var post = _db.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            _db.Posts.Remove(post);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }

}