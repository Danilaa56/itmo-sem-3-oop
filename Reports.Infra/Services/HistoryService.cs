using System;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;

namespace Reports.Infra.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ReportsContext _context;

        public HistoryService(ReportsContext context)
        {
            _context = context;
        }

        public void CreatedProblem(Guid problemId, Guid actor)
        {
            HistoryRecord historyRecord = Blank(problemId, actor);
            historyRecord.Type = HistoryRecord.RecordType.CreatedProblem;
            AddAndSave(historyRecord);
        }

        public void EditedProblem(Guid problemId, Guid actor)
        {
            HistoryRecord historyRecord = Blank(problemId, actor);
            historyRecord.Type = HistoryRecord.RecordType.EditedProblem;
            AddAndSave(historyRecord);
        }

        public void SetProblemState(Guid problemId, Guid actor, Problem.ProblemState state)
        {
            HistoryRecord historyRecord = Blank(problemId, actor);
            historyRecord.Type = HistoryRecord.RecordType.ChangedProblemState;
            AddAndSave(historyRecord);
        }

        public void AddedComment(Guid problemId, Guid actor)
        {
            HistoryRecord historyRecord = Blank(problemId, actor);
            historyRecord.Type = HistoryRecord.RecordType.AddedComment;
            AddAndSave(historyRecord);
        }

        public void EditedComment(Guid problemId, Guid actor)
        {
            HistoryRecord historyRecord = Blank(problemId, actor);
            historyRecord.Type = HistoryRecord.RecordType.EditedComment;
            AddAndSave(historyRecord);
        }

        public void SetExecutor(Guid problemId, Guid actor, Guid executor)
        {
            HistoryRecord historyRecord = Blank(problemId, actor);
            historyRecord.Type = HistoryRecord.RecordType.SetExecutor;
            AddAndSave(historyRecord);
        }

        private HistoryRecord Blank(Guid problemId, Guid actorId)
        {
            return new HistoryRecord
            {
                AffectedId = problemId,
                ActorId = actorId,
                Date = DateTime.Now,
                Type = HistoryRecord.RecordType.EditedProblem,
            };
        }

        private void AddAndSave(HistoryRecord historyRecord)
        {
            _context.History.Add(historyRecord);
            _context.SaveChanges();
        }
    }
}