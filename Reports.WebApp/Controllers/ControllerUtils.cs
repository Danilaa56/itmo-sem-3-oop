using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reports.Core.Entities;

namespace Reports.WebApp.Controllers
{
    public static class ControllerUtils
    {
        public static List<SelectListItem> SelectedListOfDirectors(Person? person, IEnumerable<Person> persons)
        {
            person ??= new Person();
            var options = persons.Where(p => p != person)
                .Select(p => new SelectListItem($"{p.Name} {p.Surname}", p.Id.ToString(), p == person.Director))
                .ToList();
            options.Insert(0, new SelectListItem("Not selected", "", person.Director == null));
            return options;
        }
    }
}