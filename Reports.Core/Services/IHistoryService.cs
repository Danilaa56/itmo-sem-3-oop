using System;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface IHistoryService
    {
        void CreatedProblem(Guid problemId, Guid actor);
        void EditedProblem(Guid problemId, Guid actor);
        void SetProblemState(Guid problemId, Guid actor, Problem.ProblemState state);
        void AddedComment(Guid problemId, Guid actor);
        void EditedComment(Guid problemId, Guid actor);
        void SetExecutor(Guid problemId, Guid actor, Guid executor);
    }
}