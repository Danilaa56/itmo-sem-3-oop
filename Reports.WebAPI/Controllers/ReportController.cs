using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Services;
using Reports.WebAPI.Models;

namespace Reports.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IReportService _reportService;

        public ReportController(IAuthService authService, IReportService reportService)
        {
            _authService = authService;
            _reportService = reportService;
        }

        [HttpPost]
        public Guid InitReport(Guid sprintId, Guid token)
        {
            return _reportService.InitReport(sprintId, _authService.PersonByToken(token));
        }

        [HttpGet("{id}")]
        public ReportModel GetById(Guid id)
        {
            return ReportModel.ToModel(_reportService.GetReportById(id));
        }

        [HttpPut("{id}")]
        public void Edit(Guid id, string content, Guid token)
        {
            _reportService.Edit(id, content, _authService.PersonByToken(token));
        }

        [HttpPut("{id}/BakeReport")]
        public void BakeReport(Guid id, Guid token)
        {
            _reportService.BakeReport(id, _authService.PersonByToken(token));
        }

        [HttpPost("{id}/LinkProblem")]
        public void LinkProblem(Guid id, Guid problemId, Guid token)
        {
            _reportService.LinkProblem(id, problemId, _authService.PersonByToken(token));
        }

        [HttpPost("{id}/UnlinkProblem")]
        public void UnlinkProblem(Guid id, Guid problemId, Guid token)
        {
            _reportService.UnlinkProblem(id, problemId, _authService.PersonByToken(token));
        }

        [HttpPost("{id}/LinkReport")]
        public void LinkReport(Guid id, Guid reportId, Guid token)
        {
            _reportService.LinkReport(id, reportId, _authService.PersonByToken(token));
        }

        [HttpPost("{id}/UnlinkReport")]
        public void UnlinkReport(Guid id, Guid reportId, Guid token)
        {
            _reportService.UnlinkReport(id, reportId, _authService.PersonByToken(token));
        }

        [HttpGet("FindByDirectorId")]
        public IEnumerable<ReportModel> FindByDirectorId(Guid directorId, Guid sprintId)
        {
            return _reportService.FindReportsByDirector(directorId, sprintId).ToList().ConvertAll(ReportModel.ToModel);
        }

        [HttpGet("FindWorkersWithoutReport")]
        public IEnumerable<PersonModel> FindWorkersWithoutReport(Guid directorId, Guid sprintId)
        {
            return _reportService.FindWorkersWithoutReport(directorId, sprintId).ToList().ConvertAll(PersonModel.ToModel);
        }
    }
}