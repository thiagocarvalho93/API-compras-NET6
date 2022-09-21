using ApiDotnet.Domain.Validations;

namespace ApiDotnet.Domain.Entities
{
    public class Purchase
    {

        #region Atributos
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public int PersonId { get; private set; }
        public DateTime Date { get; private set; }
        public Person Person { get; set; }
        public Product Product { get; set; }
        #endregion

        #region Construtores
        public Purchase()
        {
        }

        public Purchase(int productId, int personId)
        {
            Validate(productId, personId);
            ProductId = productId;
            PersonId = personId;
            Date = DateTime.Now;
        }

        public Purchase(int id, int productId, int personId)
        {
            DomainValidationException.When(id <= 0, "id inválido!");
            Validate(productId, personId);
            Id = id;
            ProductId = productId;
            PersonId = personId;
            Date = DateTime.Now;
        }
        #endregion

        #region Métodos
        private void Validate(int productId, int personId)
        {
            DomainValidationException.When(productId <= 0, "Id produto inválido!");
            DomainValidationException.When(personId <= 0, "Id pessoa inválido!");
        }

        #endregion

    }
}