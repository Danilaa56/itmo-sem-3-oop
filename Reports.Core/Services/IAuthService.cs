using System;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface IAuthService
    {
        Guid CreateToken(Guid id, string keyWord);
        Person PersonByToken(Guid tokenValue);
    }
}