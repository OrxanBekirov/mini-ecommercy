using AutoMapper;
using Business.Dtos.Brand;
using Business.Dtos.Cart;
using Business.Dtos.CartItem;
using Business.Dtos.Category;
using Business.Dtos.Media;
using Business.Dtos.Order;
using Business.Dtos.OrderItem;
using Business.Dtos.Payment;
using Business.Dtos.Product;
using Business.Dtos.Wishlist;
using DAL.Migrations;
using Entities.Concrete;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Brand
        CreateMap<Brand, BrandGetDto>();
        CreateMap<BrandCreateDto, Brand>();
        CreateMap<BrandUpdateDto, Brand>();

        // Category
        CreateMap<Category, CategoryGetDto>();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();

        // Product
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();

        // TƏK VƏ TAM PRODUCT MAPPING
        CreateMap<Product, ProductGetDto>()
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.ImageUrls, o => o.MapFrom(s =>
                (s.ProductImages ?? new List<ProductImage>())
                .OrderByDescending(i => i.IsMain)
                .Select(i => i.ImageUrl)
                .ToList()));

        // Cart
        CreateMap<Cart, CartGetDto>()
            .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity)));

        // CART ITEM MAPPING (Şəkil bura əlavə olundu!)
        CreateMap<CartItem, CartItemGetDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s =>
                s.Product.ProductImages != null && s.Product.ProductImages.Any()
                ? s.Product.ProductImages.OrderByDescending(x => x.IsMain).FirstOrDefault().ImageUrl
                : null));

        // Order & Others
        CreateMap<Order, OrderGetDto>().ForMember(d => d.OrderItems, o => o.MapFrom(s => s.OrderItems));
        CreateMap<OrderItem, OrderItemGetDto>().ForMember(d => d.LineTotal, o => o.MapFrom(s => s.UnitPrice * s.Quantity));
        CreateMap<Payment, PaymentGetDto>();
        CreateMap<Media, MediaUploadResultDto>().ReverseMap();
        CreateMap<Wishlist, WishlistGetDto>()
      .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
      .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Product.Category.Name))
      .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Product.Brand.Name))
      .ForMember(d => d.Price, o => o.MapFrom(s => s.Product.Price))
      .ForMember(d => d.ImageUrls, o => o.MapFrom(s =>
          s.Product.ProductImages != null && s.Product.ProductImages.Any()
          ? s.Product.ProductImages.Select(x => x.ImageUrl).ToList()
          : new List<string>()));
    }
}
