using Business.Dtos.Product;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Product
{
    public class ProductUpdateDtoValidator: AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name boş ola bilməz")
                .MinimumLength(2).WithMessage("Name minimum 2 simvol olmalıdır")
                .MaximumLength(100).WithMessage("Name maksimum 100 simvol olmalıdır");

            RuleFor(x => x.Description)
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

