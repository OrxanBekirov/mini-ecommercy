using Business.Dtos.Product;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Product
{
    public class ProductImageCreateDtoValidator:AbstractValidator<ProductImageCreateDto>
    {
        public ProductImageCreateDtoValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Url).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.PublicId).NotEmpty().MaximumLength(200);
        }
    }
}
