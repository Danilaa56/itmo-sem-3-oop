using System;
using Reports.Core.Entities;

namespace Reports.WebAPI.Models
{
    public class HistoryRecordModel
    {
        public Guid Id { get; set; }
        public Guid AffectedId { get; set; }
        public Guid ActorId { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }

        public static HistoryRecordModel ToModel(HistoryRecord historyRecord)
        {
            return new HistoryRecordModel
            {
                Id = historyRecord.Id,
                AffectedId = historyRecord.AffectedId,
                ActorId = historyRecord.ActorId,
                Type = historyRecord.Type.ToString(),
                Date = historyRecord.Date
            };
        }
    }
}