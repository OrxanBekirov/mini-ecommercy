using Business.Dtos.Brand;
using Business.Dtos.Category;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface ICategoryService
    {

        Task<IDataResult<List<CategoryGetDto>>> GetAllAsync();
        Task<IDataResult<CategoryGetDto>> GetByIdAsync(int id);

        Task<IResult> AddAsync(CategoryCreateDto dto);
        Task<IResult> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<IResult> DeleteAsync(int id);
       Task<IDataResult<List<CategoryGetDto>>> GetAllPaginationAsync(int pageNumber, int pageSize);
    }
}
