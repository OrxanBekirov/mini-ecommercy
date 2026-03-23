using Business.Dtos.Cart;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Cart
{
    public class CartAddDtoValidator:AbstractValidator<CartAddDto>
    {
        public CartAddDtoValidator()
        {

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId düzgün deyil");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity 0-dan böyük olmalıdır")
                .LessThanOrEqualTo(100).WithMessage("Quantity maksimum 100 ola bilər");
        }
    }
}
