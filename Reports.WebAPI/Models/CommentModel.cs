using System;
using Reports.Core.Entities;

namespace Reports.WebAPI.Models
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public static CommentModel ToModel(Comment comment)
        {
            return new CommentModel
            {
                Id = comment.Id,
                Content = comment.Content,
                Created = comment.Created,
                Updated = comment.Updated,
                AuthorId = comment.Author.Id
            };
        }
    }
}