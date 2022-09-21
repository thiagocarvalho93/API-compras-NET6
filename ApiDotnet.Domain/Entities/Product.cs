using ApiDotnet.Domain.Validations;

namespace ApiDotnet.Domain.Entities
{
    public sealed class Product
    {

        #region Atributos
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string CodErp { get; private set; }
        public decimal Price { get; private set; }
        public ICollection<Purchase> Purchases { get; set; }

        #endregion

        #region Construtores
        public Product(string name, string codErp, decimal price)
        {
            Validate(name, codErp, price);
            Name = name;
            CodErp = codErp;
            Price = price;
            Purchases = new List<Purchase>();
        }

        public Product(int id, string name, string codErp, decimal price)
        {
            DomainValidationException.When(id < 0, "id inválido!");
            Validate(name, codErp, price);
            Id = id;
            Name = name;
            CodErp = codErp;
            Price = price;
            Purchases = new List<Purchase>();
        }


        #endregion

        private void Validate(string name, string codErp, decimal price)
        {
            DomainValidationException.When(string.IsNullOrEmpty(name), "Nome deve ser informado!");
            DomainValidationException.When(string.IsNullOrEmpty(codErp), "Código erp deve ser informado!");
            DomainValidationException.When(price < 0, "Preço inválido!");
        }

    }
}