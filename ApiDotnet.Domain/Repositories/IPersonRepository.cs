using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDotnet.Domain.Entities;

namespace ApiDotnet.Domain.Repositories
{
    public interface IPersonRepository
    {
        Task<Person> GetByIdAsync(int id);
        Task<ICollection<Person>> GetPeopleAsync();
        Task<Person> CreateAsync(Person person);
        Task EditAsync(Person person);
        Task DeleteAsync(Person person);
    }
}