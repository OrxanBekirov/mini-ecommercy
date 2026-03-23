using Business.Dtos.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Order
{
    public class OrderStatusUpdateDtoValidator:AbstractValidator<OrderStatusUpdateDto>
    {
        public OrderStatusUpdateDtoValidator()
        {
            RuleFor(x => x.Status)
           .IsInEnum()
           .WithMessage("OrderStatus düzgün deyil.");
        }
    }
}
