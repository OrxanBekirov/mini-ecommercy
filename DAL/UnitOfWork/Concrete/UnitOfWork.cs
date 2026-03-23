using DAL.Repositories.Abstract;
using DAL.Repositories.Concrete;
using DAL.UnitOfWork.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.UnitOfWork.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly  IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IWishlistRepository _wishlistRepository;

        private readonly CommercyDbContext _context;

        public UnitOfWork(CommercyDbContext context)
        {
            _context = context;
        }

        public IBrandRepository BrandRepository => _brandRepository ?? (IBrandRepository)new EfBrandRepository(_context);
        public ICategoryRepository CategoryRepository => _categoryRepository ?? (ICategoryRepository)new EfCategoryRepository(_context);

        public ICartRepository CartRepository => _cartRepository ?? (ICartRepository)new EfCartRepository(_context);

        public ICartItemRepository CartitemRepository =>_cartItemRepository ?? (ICartItemRepository)new EfCartItemRepository(_context);

        public IProductRepository ProductRepository => _productRepository ?? (IProductRepository)new EfProductRepository(_context);

        public IOrderRepository OrderRepository =>_orderRepository ?? (IOrderRepository)new EfOrderRepository(_context);

        public IOrderItemRepository OrderItemRepository => _orderItemRepository ?? (IOrderItemRepository)new EfOrderItemRepository(_context);

        public IPaymentRepository PaymentRepository => _paymentRepository ?? (IPaymentRepository)new EfPaymentrepository(_context);

        public IProductImageRepository ProductImageRepository => _productImageRepository ?? (IProductImageRepository)new EfProductImagerepository(_context);

        public IMediaRepository MediaRepository =>_mediaRepository ?? (IMediaRepository)new EfMediaRepository(_context);

        public IWishlistRepository WishlistRepository => _wishlistRepository ?? (IWishlistRepository)new EfWishListRepository(_context);

        public Task<int> SaveChangesAsync()
        {
          return _context.SaveChangesAsync();
        }
    }
}
