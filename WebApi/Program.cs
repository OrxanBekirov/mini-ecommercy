using Business.Abstract;
using Business.Concrete;
using Business.Configuration;
using Business.Helpers.Settings;
using Business.Validators.Cart;
using Business.Validators.Contact;
using Business.Validators.Order;
using Business.Validators.Payment;
using Business.Validators.Product;
using CloudinaryDotNet;
using DAL;

using DAL.Repositories.Abstract;
using DAL.Repositories.Concrete;
using DAL.UnitOfWork.Abstract;
using DAL.UnitOfWork.Concrete;
using Entities.Concrete.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Stripe;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using WebApi;
using WebApi.Configuration;
using WebApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// 1. TokenOption
var tokenOption = builder.Configuration.GetSection("TokenOption").Get<TokenOption>()
    ?? new TokenOption { SecurityKey = "temporary_key_for_build_32_chars_min", Issuer = "test", Audience = "test" };
builder.Services.AddSingleton(tokenOption);

// DbContext (PostgreSQL)
builder.Services.AddDbContext<CommercyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// EmailSettings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Services & Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBrandService, BrandManager>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<ITokenService, TokenManager>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<IOrderService, OrderManager>();
builder.Services.AddScoped<IPaymentService, PaymentManager>();
builder.Services.AddScoped<IMediaService, MediaManager>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IWishlistService, WishlistManager>();
builder.Services.AddScoped<IEmailService, EmailManager>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(ProductManager).Assembly);
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Stripe
var stripeSection = builder.Configuration.GetSection("StripeSettings");
var stripeSettings = stripeSection.Get<StripeSetting>() ?? new StripeSetting { SecretKey = "sk_test_temp" };
StripeConfiguration.ApiKey = stripeSettings.SecretKey;
builder.Services.Configure<StripeSetting>(stripeSection);

// Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new CloudinaryDotNet.Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
    );
    return new Cloudinary(account);
});

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<OrderCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ContactSendDtoValidator>();

// Identity
builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<CommercyDbContext>()
    .AddDefaultTokenProviders();

// Authentication & JWT
builder.Services.Configure<TokenOption>(builder.Configuration.GetSection("TokenOption"));
builder.Services.AddScoped<ITokenService, TokenManager>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenOption.Issuer,
        ValidAudience = tokenOption.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecurityKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ✅ CORS (Düzgün)
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new string[] { "https://mini-ecommercy-front.vercel.app" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        );
});

var app = builder.Build();

// 1. MÜTLƏQ BU SIRALAMA OLMALIDIR:
app.UseCors("MyCorsPolicy");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// Auth hər zaman CORS-dan sonra gəlir
app.UseAuthentication();
app.UseAuthorization();


// ✅ CORS çağırışı həmişə authentication-dan əvvəl



// Migration və seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CommercyDbContext>();

    await context.Database.MigrateAsync();
    await SeedData.SeedRolesAndAdminAsync(services);
}

// Webhook buffering
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value.Contains("webhook"))
    {
        context.Request.EnableBuffering();
    }
    await next();
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.MapControllers();
app.Run();