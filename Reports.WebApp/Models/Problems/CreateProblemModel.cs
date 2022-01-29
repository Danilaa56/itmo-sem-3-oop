using System.ComponentModel.DataAnnotations;

namespace Reports.WebApp.Models.Problems
{
    public class CreateProblemModel
    {
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}