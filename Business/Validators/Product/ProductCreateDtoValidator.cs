using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Business.Dtos.Product;
namespace Business.Validators.Product
{
    public class ProductCreateDtoValidator:AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            
        RuleFor(x=>x.Description)
            .MaximumLength(1000).WithMessage("Description maksimum 1000 simvol olmalıdır");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price 0-dan böyük olmalıdır");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("StockQuantity mənfi ola bilməz");

        RuleFor(x => x.BrandId)
            .GreaterThan(0).WithMessage("BrandId düzgün deyil");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId düzgün deyil");
        }
    }
}
