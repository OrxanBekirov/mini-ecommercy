using Business.Dtos.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Order
{
    public class OrderCancelDtoValidator:AbstractValidator<OrderCancelDto>
    {
        public OrderCancelDtoValidator()
        {
            RuleFor(x => x.Reason)
           .MaximumLength(500);
        }
    }
}
