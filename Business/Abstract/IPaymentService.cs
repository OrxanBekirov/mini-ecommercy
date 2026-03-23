using Business.Dtos.Payment;
using Core.Result.Abstract;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Text;
namespace Business.Abstract
{
    public interface IPaymentService
    {
        Task<IDataResult<PaymentGetDto>> MarkSuccessAsync(string userId, PaymentMarkSuccessDto dto);
        Task<IDataResult<PaymentGetDto>> MarkFailedAsync(string userId, PaymentMarkFailedDto dto);
        Task<IDataResult<CheckoutSessionResponseDto>> CreateCheckoutSessionAsync(CreateCheckoutSessionDto dto);
        Task HandleCheckoutSessionCompletedAsync(Session session);
        Task HandlePaymentIntentSucceededAsync(PaymentIntent intent);
    }
}
