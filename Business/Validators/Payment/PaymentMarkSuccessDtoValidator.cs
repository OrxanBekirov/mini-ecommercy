using Business.Dtos.Payment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Payment
{
    public class PaymentMarkSuccessDtoValidator:AbstractValidator<PaymentMarkSuccessDto>
    {
        public PaymentMarkSuccessDtoValidator()
        {
            RuleFor(x => x.OrderId)
           .GreaterThan(0)
           .WithMessage("OrderId 0-dan böyük olmalıdır.");

            RuleFor(x => x.ProviderReference)
                .MaximumLength(200)
                .WithMessage("ProviderReference maksimum 200 simvol ola bilər.");
        }
    }
}
