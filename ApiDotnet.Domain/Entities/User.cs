using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDotnet.Domain.Validations;

namespace ApiDotnet.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public User(string email, string password)
        {
            Validate(email, password);
            Email = email;
            Password = password;
        }

        public User(int id, string email, string password)
        {
            Validate(email, password);
            Id = id;
            Email = email;
            Password = password;
        }

        private void Validate(string email, string password)
        {
            DomainValidationException.When(string.IsNullOrEmpty(email), "Email deve ser informado.");
            DomainValidationException.When(string.IsNullOrEmpty(password), "Senha deve ser informada.");
        }
    }
}