using Business.Dtos.Order;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IOrderService
    {
        Task<IDataResult<List<OrderGetDto>>> GetAllOrdersAsync();
        Task<IDataResult<OrderGetDto>> CreateAsync(string userId, OrderCreateDto dto);
        Task<IDataResult<OrderGetDto>> GetByIdAsync(int id, string userId);
        

        Task<IDataResult<List<OrderGetDto>>> GetMyOrdersAsync(string userId);
        Task<IDataResult<OrderGetDto>> UpdateStatusAsync(int orderId, OrderStatusUpdateDto dto);
        Task<IDataResult<OrderGetDto>> CreateFromCartAsync(string userId, OrderFromCartDto dto);
        Task<IDataResult<OrderGetDto>> CancelAsync(int orderId,string userid,OrderCancelDto dto);
        Task<IResult> UpdatePaymentStatusAsync(int orderId, bool isSuccess, string providerReference);
    }
}
