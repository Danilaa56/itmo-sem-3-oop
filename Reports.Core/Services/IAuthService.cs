using System;

namespace Reports.Core.Services
{
    public interface IAuthService
    {
        Guid Register(string username, string password, string name, string surname);
        Guid Register(string username, string password, string name, string surname, Guid directorId);
        Guid Login(string username, string password);
    }
}