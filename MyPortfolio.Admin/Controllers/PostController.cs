﻿    using MyPortfolio.Model;
using MyPortfolio.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPortfolio.Admin.Controllers
{
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly ICategoryService categoryService;
        public PostController(IPostService postService, ICategoryService categoryService, INotificationService notificationService) : base(notificationService)
        {
            this.postService = postService;
            this.categoryService = categoryService;
        }
        // GET: Post
        public ActionResult Index( )
        {
            var post = postService.GetAll();
            return View(post);
        }
        public ActionResult Create()
        {
            var post = new Post();
            ViewBag.CategoryId = new SelectList(categoryService.GetAll(), "Id", "Name");
            return View(post);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Post post, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(upload.FileName);
                    string extension = Path.GetExtension(fileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif") { 
                        string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                        upload.SaveAs(path);
                        post.Photo = fileName;
                        postService.Insert(post);
                        return RedirectToAction("index");
                    } else
                    {
                        ModelState.AddModelError("Photo", "Dosya uzantısı .jpg, .jpeg, .png ya da .gif olmalıdır.");
                    }
                }
                
            }
            ViewBag.CategoryId = new SelectList(categoryService.GetAll(), "Id", "Name", post.CategoryId);
            return View(post);
        }
        public ActionResult Edit(Guid id)
        {
            var post = postService.Find(id);
            if (post == null)
            {
                return HttpNotFound();

            }
            ViewBag.CategoryId = new SelectList(categoryService.GetAll(), "Id", "Name", post.CategoryId);
            return View(post);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                postService.Update(post);
                return RedirectToAction("index");

            }
            ViewBag.CategoryId = new SelectList(categoryService.GetAll(), "Id", "Name", post.CategoryId);
            return View(post);
        }
        public ActionResult Delete(Guid id)
        {
            postService.Delete(id);
            return RedirectToAction("index");
        }
       
        public ActionResult Details(Guid id)
        {
            var post = postService.Find(id);
            if (post == null)
            {
                return HttpNotFound();

            }
           
            return View(post);
        }
    }
}