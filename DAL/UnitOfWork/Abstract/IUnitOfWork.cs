using DAL.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.UnitOfWork.Abstract
{
    public interface IUnitOfWork
    {
         IBrandRepository  BrandRepository { get; }
         ICategoryRepository CategoryRepository { get; }
         ICartRepository CartRepository { get; }
         ICartItemRepository CartitemRepository { get; }
         IProductRepository ProductRepository { get; }



         IMediaRepository MediaRepository { get; }

         IOrderRepository OrderRepository { get; }
         IOrderItemRepository OrderItemRepository { get; }

        public IPaymentRepository PaymentRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }
        IWishlistRepository WishlistRepository { get; }
        public Task<int> SaveChangesAsync();
    }
}
