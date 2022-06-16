using System;
using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.WebAPI.Models
{
    public class ProblemModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Problem.ProblemState State { get; set; }
        public Guid SprintId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid ExecutorId { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }

        public static ProblemModel ToModel(Problem problem)
        {
            return new ProblemModel
            {
                Id = problem.Id,
                Title = problem.Title,
                Content = problem.Content,
                Created = problem.Created,
                Updated = problem.Updated,
                State = problem.State,
                SprintId = problem.Sprint.Id,
                AuthorId = problem.Author.Id,
                ExecutorId = problem.Executor?.Id ?? Guid.Empty,
                Comments = problem.Comments.ConvertAll(CommentModel.ToModel)
            };
        }
    }
}