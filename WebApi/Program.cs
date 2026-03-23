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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;// Bu çox vacibdir
using Stripe;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection; 
using System.Text;
using WebApi;
using WebApi.Configuration;
using WebApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// 1. Öncə TokenOption-u appsettings-dən oxuyuruq
var tokenOption = builder.Configuration.GetSection("TokenOption").Get<TokenOption>()
    ?? new TokenOption { SecurityKey = "temporary_key_for_build_32_chars_min", Issuer = "test", Audience = "test" };
// 2. Oxuduğumuz bu obyekti DI konteynerinə Singleton olaraq əlavə edirik (BU ÇOX VACİBDİR)
builder.Services.AddSingleton(tokenOption);

// DbContext
builder.Services.AddDbContext<CommercyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//EmailConfiguration
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
    // Enum-ların rəqəm (0,1) yox, söz (Pending, Paid) kimi görünməsi üçün
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
// sistemin oz icinde cekir Usere olan melumati=tlari
builder.Services.AddHttpContextAccessor();

// Swagger (Authorize düyməsi ilə)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Stripe Setting
var stripeSection = builder.Configuration.GetSection("StripeSettings");
var stripeSettings = stripeSection.Get<StripeSetting>() ?? new StripeSetting { SecretKey = "sk_test_temp" };
StripeConfiguration.ApiKey = stripeSettings.SecretKey;
builder.Services.Configure<StripeSetting>(stripeSection);

if (stripeSettings == null || string.IsNullOrWhiteSpace(stripeSettings.SecretKey))
{
    throw new Exception("Stripe konfiqurasiyası tapılmadı! appsettings.json-u yoxlayın.");
}

StripeConfiguration.ApiKey = stripeSettings.SecretKey;
//Cloudinary Configuration
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

    // 'Account' yerinə tam adını yazırıq:
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
//builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());






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
// AddCors edirem 
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// burda bitdi
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CommercyDbContext>();

    // 1. ƏVVƏLCƏ CƏDVƏLLƏRİ YARAT (BU ÇOX VACİBDİR!)
    await context.Database.MigrateAsync();

    // 2. SONRA ADMİN VƏ ROLLARI ƏLAVƏ ET
    await SeedData.SeedRolesAndAdminAsync(services);
}
await SeedData.SeedRolesAndAdminAsync(app.Services);
app.Use(async (context, next) =>
{
    // Bütün webhook müraciətləri üçün buffering-i açırıq
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