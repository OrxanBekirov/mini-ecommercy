using Business.Abstract;
using Business.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.V2.Core;
using WebApi.Settings;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly StripeSetting _stripeSettings;
        private readonly IOrderService _orderService;

        public StripeWebhookController(IPaymentService paymentService, IOptions<StripeSetting> stripeOptions, IOrderService orderService)
        {
            _paymentService = paymentService;
            _stripeSettings = stripeOptions.Value;
            _orderService = orderService;
        }
        [HttpPost("webhook")]
[AllowAnonymous]
public async Task<IActionResult> Handle()
{
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

    try
    {
        var stripeSignature = Request.Headers["Stripe-Signature"];

        var stripeEvent = EventUtility.ConstructEvent(
            json,
            stripeSignature,
            _stripeSettings.WebhookSecret,
            throwOnApiVersionMismatch: false
        );

        // 1. Əgər Stripe Checkout səhifəsindən gəlirsə
        if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session != null)
            {
                await _paymentService.HandleCheckoutSessionCompletedAsync(session);
                Console.WriteLine("✅ Checkout Session ilə status yeniləndi.");
            }
        }
                // 2. Əgər birbaşa Payment Intent uğurlu olubsa (Trigger edəndə bu gəlir)
                else if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var intent = stripeEvent.Data.Object as Stripe.PaymentIntent;
                    if (intent != null)
                    {
                        // Artıq xəta verməyəcək, çünki metod PaymentIntent qəbul edir
                        await _paymentService.HandlePaymentIntentSucceededAsync(intent);
                        Console.WriteLine("✅ Payment Intent hadisəsi emal olundu.");
                    }
                }

                return Ok();
    }
    catch (StripeException e)
    {
        Console.WriteLine($"❌ Stripe Xətası: {e.Message}");
        return BadRequest();
    }
}
    }
}