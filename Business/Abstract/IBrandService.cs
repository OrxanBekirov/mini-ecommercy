using Business.Dtos.Brand;
using Core.Result.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IBrandService
    {

        Task<IDataResult<List<BrandGetDto>>> GetAllAsync();
        Task<IDataResult<BrandGetDto>> GetByIdAsync(int id);

        Task<IResult> AddAsync(BrandCreateDto dto);
        Task<IResult> UpdateAsync(int id, BrandUpdateDto dto);
        Task<IResult> DeleteAsync(int id);

    }
}
