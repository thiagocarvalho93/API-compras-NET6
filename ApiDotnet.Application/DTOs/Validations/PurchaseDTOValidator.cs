using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace ApiDotnet.Application.DTOs.Validations
{
    public class PurchaseDTOValidator : AbstractValidator<PurchaseDTO>
    {
        public PurchaseDTOValidator()
        {
            RuleFor(x => x.CodErp)
                .NotNull()
                .NotEmpty()
                .WithMessage("Código Erp deve ser informado.");

            RuleFor(x => x.Document)
                .NotEmpty()
                .NotNull()
                .WithMessage("Documento deve ser informado");
        }
    }
}