using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace ApiDotnet.Application.DTOs.Validations
{
    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("Nome deve ser informado");

            RuleFor(x => x.Price)
                .NotEmpty()
                .NotNull()
                .WithMessage("Preço deve ser informado");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Preço deve ser maior que zero.");

            RuleFor(x => x.CodErp)
                .NotEmpty()
                .NotNull()
                .WithMessage("Código ERP deve ser informado.");


        }
    }
}