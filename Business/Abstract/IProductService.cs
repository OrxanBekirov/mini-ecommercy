using Business.Dtos.Product;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IProductService
    {
        Task<IDataResult<List<ProductGetDto>>> GetAllAsync();
        Task<IDataResult<List<ProductGetDto>>> GetAllPaginatedAsync(int pageNumber, int pageSize);
        Task<IDataResult<ProductGetDto>> GetByIdAsync(int id);
        Task<IResult> AddAsync(ProductCreateDto dto);
        Task<IResult> UpdateAsync(int id, ProductUpdateDto dto);
        Task<IResult> DeleteAsync(int id);
    }
}
