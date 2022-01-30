using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.WebAPI.Models;

namespace Reports.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProblemController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IProblemService _problemService;

        public ProblemController(IProblemService problemService, IAuthService authService)
        {
            _problemService = problemService;
            _authService = authService;
        }

        [HttpGet]
        public IEnumerable<ProblemModel> List()
        {
            return _problemService.GetProblemsList().Select(ProblemModel.ToModel);
        }

        [HttpGet("{id}")]
        public ProblemModel GetById(Guid id)
        {
            return ProblemModel.ToModel(_problemService.GetProblemById(id));
        }

        [HttpPost]
        public void CreateProblem(string title, string content, Guid sprintId, Guid token)
        {
            _problemService.CreateProblem(title, content, sprintId, _authService.PersonByToken(token));
        }

        [HttpPut("{id}")]
        public void Edit(Guid id, string title, string content, Guid sprintId, Guid token)
        {
            _problemService.EditProblem(id, title, content, sprintId, _authService.PersonByToken(token));
        }

        [HttpPut("{id}/setState")]
        public void SetState(Guid id, Problem.ProblemState state, Guid token)
        {
            _problemService.SetState(id, state, _authService.PersonByToken(token));
        }

        [HttpPost("{id}/addComment")]
        public void AddComment(Guid id, Guid authorId, string content, Guid token)
        {
            _problemService.AddComment(id, content, _authService.PersonByToken(token));
        }
    }
}