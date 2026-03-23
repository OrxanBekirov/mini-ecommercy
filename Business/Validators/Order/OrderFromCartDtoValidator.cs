using Business.Dtos.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Order
{
    public class OrderFromCartDtoValidator:AbstractValidator<OrderFromCartDto>
    {
        public OrderFromCartDtoValidator()
        {
            RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Note).MaximumLength(500);
            RuleFor(x => x.PaymentMethod).IsInEnum();
        }
    }
}
