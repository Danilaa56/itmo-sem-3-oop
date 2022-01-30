using System;

namespace Reports.Core.Services
{
    public interface IReportService
    {
        Guid CreateReportDraft(Guid sprintId);
    }
}