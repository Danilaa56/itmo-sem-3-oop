using System;

namespace Reports.Core.Entities
{
    public class HistoryRecord
    {

        public enum RecordType
        {
            CreatedProblem,
            EditedProblem,
            ChangedProblemState,
            AddedComment,
            EditedComment,
            SetExecutor,
        }

        public Guid Id { get; set; }
        public Guid AffectedId { get; set; }
        public Guid ActorId { get; set; }
        public RecordType Type { get; set; }
        public DateTime Date { get; set; }
    }
}