using Business.Dtos.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Order
{
    public class OrderCreateDtoValidator: AbstractValidator<OrderCreateDto>
    
    {
        public OrderCreateDtoValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().MaximumLength(500);

            RuleFor(x => x.Note)
                .MaximumLength(500);

            RuleFor(x => x.Items)
                .NotNull()
                .Must(x => x.Count > 0).WithMessage("Ən az 1 item olmalıdır.");

            RuleForEach(x => x.Items).SetValidator(new OrderItemCreateDtoValidator());
        
        }

    }
}
