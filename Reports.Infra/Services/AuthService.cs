using System;
using System.Linq;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;
using Reports.Infra.Tools;

namespace Reports.Infra.Services
{
    public class AuthService : IAuthService
    {
        private readonly ReportsContext _context;
        private readonly IPersonService _personService;

        public AuthService(ReportsContext context, IPersonService personService)
        {
            _context = context;
            _personService = personService;
        }

        public Guid CreateToken(Guid id, string keyWord)
        {
            if (!_context.Persons.Any(person => person.Id.Equals(id)))
                throw new AuthException($"There is no person with id {id}");
            Token? token = _context.Tokens.FirstOrDefault(token => token.Id == id);
            if (token is null)
            {
                token = new Token
                {
                    Id = id,
                    KeyWord = keyWord,
                    TokenValue = new Guid(56, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                    // TokenValue = Guid.NewGuid()
                };
                _context.Tokens.Add(token);
            }
            else if (!token.KeyWord.Equals(keyWord))
            {
                throw new AuthException("Wrong credentials");
            }
            else
            {
                token.TokenValue = Guid.NewGuid();
                _context.Tokens.Update(token);
            }

            _context.SaveChanges();
            return token.TokenValue;
        }

        public Person PersonByToken(Guid tokenValue)
        {
            Token token = _context.Tokens.FirstOrDefault(token => token.TokenValue.Equals(tokenValue))
                          ?? throw new AuthException("Invalid token");
            return _personService.GetPersonById(token.Id);
        }
    }
}