using ApiDotnet.Domain.Repositories;

namespace ApiDotnet.Domain.FiltersDb
{
    public class PersonFilterDb : PageBasedRequest
    {
        public string Name { get; set; }
    }
}