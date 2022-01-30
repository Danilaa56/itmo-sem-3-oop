using System;
using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface IProblemService
    {
        Guid CreateProblem(string title, string content, Guid sprintId, Person actor);
        Problem GetProblemById(Guid id);
        IEnumerable<Problem> GetProblemsList();
        IEnumerable<Problem> FindProblemsCreatedInPeriod(DateTime since, DateTime upto);
        IEnumerable<Problem> FindProblemsUpdatedInPeriod(DateTime since, DateTime upto);
        IEnumerable<Problem> FindProblemsByExecutor(Guid executorId);
        IEnumerable<Problem> FindProblemsByExecutorDirector(Guid directorId);
        IEnumerable<Problem> FindProblemsBySprint(Guid sprintId);
        void EditProblem(Guid problemId, string newTitle, string newContent, Guid newSprintId, Person actor);
        void SetState(Guid problemId, Problem.ProblemState state, Person actor);
        void SetExecutor(Guid problemId, Guid personId, Person actor);
        void AddComment(Guid problemId, string content, Person actor);
        void EditComment(Guid problemId, Guid commentId, string newContent, Person actor);
    }
}