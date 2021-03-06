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
        private readonly IHistoryService _historyService;
        private readonly IProblemService _problemService;

        public ProblemController(IProblemService problemService, IAuthService authService, IHistoryService historyService)
        {
            _problemService = problemService;
            _authService = authService;
            _historyService = historyService;
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

        [HttpPut("{id}/setExecutor")]
        public void SetExecutor(Guid id, Guid executorId, Guid token)
        {
            _problemService.SetExecutor(id, executorId, _authService.PersonByToken(token));
        }

        [HttpPost("{id}/addComment")]
        public void AddComment(Guid id, Guid authorId, string content, Guid token)
        {
            _problemService.AddComment(id, content, _authService.PersonByToken(token));
        }

        [HttpGet("{id}/getHistory")]
        public IEnumerable<HistoryRecordModel> GetHistory(Guid id)
        {
            return _historyService.GetHistory(id).Select(HistoryRecordModel.ToModel);
        }

        [HttpGet("FindByTimeCreated")]
        public IEnumerable<ProblemModel> FindByTimeCreated(DateTime since, DateTime upto)
        {
            return _problemService.FindProblemsCreatedInPeriod(since, upto).Select(ProblemModel.ToModel);
        }

        [HttpGet("FindByTimeUpdated")]
        public IEnumerable<ProblemModel> FindByTimeUpdated(DateTime since, DateTime upto)
        {
            return _problemService.FindProblemsUpdatedInPeriod(since, upto).Select(ProblemModel.ToModel);
        }

        [HttpGet("FindByExecutor")]
        public IEnumerable<ProblemModel> FindByTimeCreated(Guid executorId)
        {
            return _problemService.FindProblemsByExecutor(executorId).Select(ProblemModel.ToModel);
        }

        [HttpGet("FindByPersonEdited")]
        public IEnumerable<ProblemModel> FindByPersonEdited(Guid personId)
        {
            return _problemService.FindByPersonEdited(personId).Select(ProblemModel.ToModel);
        }

        [HttpGet("FindByExecutorDirector")]
        public IEnumerable<ProblemModel> FindByExecutorDirector(Guid directorId)
        {
            return _problemService.FindProblemsByExecutorDirector(directorId).Select(ProblemModel.ToModel);
        }

        [HttpGet("FindBySprint")]
        public IEnumerable<ProblemModel> FindBySprint(Guid sprintId)
        {
            return _problemService.FindProblemsBySprint(sprintId).Select(ProblemModel.ToModel);
        }
    }
}