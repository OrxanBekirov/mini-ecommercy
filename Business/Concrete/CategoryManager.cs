using AutoMapper;
using Busines.Utilities.Contants;
using Business.Abstract;
using Business.Dtos.Category;
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult> AddAsync(CategoryCreateDto dto)
        {
            if (await _unitOfWork.CategoryRepository
               .IsExistAsync(p => p.Name == dto.Name))
            {
                return new ErrorResult(ExceptionsMessage.CategoryNameAlreadyExists);
            }

            var category = _mapper.Map<Category>(dto);

            await _unitOfWork.CategoryRepository.AddAsync(category);

            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0)
                return new ErrorResult(ExceptionsMessage.AddFailed);

            return new SuccessResult(ExceptionsMessage.CategoryAdded);
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(p => p.Id == id);

            if (category is null)
                return new ErrorResult(ExceptionsMessage.CategoryNotFound);

            _unitOfWork.CategoryRepository.Remove(category);

            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0)
                return new ErrorResult(ExceptionsMessage.DeleteFailed);

            return new SuccessResult(ExceptionsMessage.CategoryDeleted);
        }

        public async Task<IDataResult<List<CategoryGetDto>>> GetAllAsync()
        {
            // BrandRepository yox, CategoryRepository olmalıdır
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            var dtos = _mapper.Map<List<CategoryGetDto>>(categories);
            return new SuccessDataResult<List<CategoryGetDto>>(dtos);
        }

        public async Task<IDataResult<List<CategoryGetDto>>> GetAllPaginationAsync(int pageNumber, int pageSize)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllWithPaginationAsync(pageNumber, pageSize,null);

            var dtos = _mapper.Map<List<CategoryGetDto>>(categories);
            return new SuccessDataResult<List<CategoryGetDto>>(dtos);
        }

        public async Task<IDataResult<CategoryGetDto>> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(p => p.Id == id);

            if (category is null)
                return new ErrorDataResult<CategoryGetDto>(null, ExceptionsMessage.CategoryNotFound);

            var dto = _mapper.Map<CategoryGetDto>(category);

            // SuccessDataResult<CategoryGetDto> olmalıdır
            return new SuccessDataResult<CategoryGetDto>(dto);
        }

        public async Task<IResult> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(p => p.Id == id);

            if (category is null)
                return new ErrorResult(ExceptionsMessage.CategoryNotFound);

            _mapper.Map(dto, category);

            _unitOfWork.CategoryRepository.Update(category);

            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0)
                return new ErrorResult(ExceptionsMessage.UpdateFailed);

            return new SuccessResult(ExceptionsMessage.CategoryUpdated);
        }
    }
}