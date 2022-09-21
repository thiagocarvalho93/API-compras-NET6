using ApiDotnet.Domain.Validations;

namespace ApiDotnet.Domain.Entities
{
    public sealed class Person
    {
        #region Atributes
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Document { get; private set; }
        public string Phone { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        #endregion

        #region Constructors
        public Person(string name, string document, string phone)
        {
            Validate(document, name, phone);
            Name = name;
            Document = document;
            Phone = phone;
            Purchases = new List<Purchase>();
        }

        public Person(int id, string name, string document, string phone)
        {
            Validate(document, name, phone);
            Id = id;
            Name = name;
            Document = document;
            Phone = phone;
            Purchases = new List<Purchase>();
        }
        #endregion

        private void Validate(string document, string name, string phone)
        {
            DomainValidationException.When(string.IsNullOrEmpty(name), "Nome deve ser informado!");
            DomainValidationException.When(string.IsNullOrEmpty(document), "Documento deve ser informado!");
            DomainValidationException.When(string.IsNullOrEmpty(phone), "Celular deve ser informado!");
        }


    }
}