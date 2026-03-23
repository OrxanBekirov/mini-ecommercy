using Business.Dtos.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Entities.Concrete;
using Business.Dtos.OrderItem;
namespace Business.Validators.Order
{
    public class OrderItemCreateDtoValidator: AbstractValidator<OrderItemCreateDto>
    {
        public OrderItemCreateDtoValidator()
        {
            RuleFor(p=>p.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(999);
        }
    }
}
