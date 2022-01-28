using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface IPersonService
    {
        IEnumerable<Person> GetPersonsList();
    }
}