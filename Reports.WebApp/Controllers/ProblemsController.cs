using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.WebApp.Models.Problems;

namespace Reports.WebApp.Controllers
{
    [Authorize]
    public class ProblemsController : Controller
    {
        private readonly IProblemService _problemService;

        public ProblemsController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Problems = _problemService.GetProblemsList();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult View(Guid id)
        {
            try
            {
                Problem problem = _problemService.GetProblemById(id);
                return View(problem);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            try
            {
                Problem problem = _problemService.GetProblemById(id);
                return View(problem);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Create(CreateProblemModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var authorId = Guid.Parse(User.Identity!.Name!);
                model.Content = model.Content.ReplaceLineEndings("\n");
                Guid problemId = _problemService.CreateProblem(model.Title, model.Content, authorId);

                return RedirectToAction("View", new {id = problemId});
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}