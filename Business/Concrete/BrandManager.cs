using AutoMapper;
using Business.Abstract;
using Business.Dtos.Brand;
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using Busines.Utilities.Contants;
using System;
using System.Collections.Generic;
using System.Text;
using Entities.Concrete;


namespace Business.Concrete
{
    public class BrandManager : IBrandService
    {   
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult> AddAsync(BrandCreateDto dto)
        {
            if (await _unitOfWork.BrandRepository
               .IsExistAsync(p => p.Name == dto.Name))
            {
                return new ErrorResult(ExceptionsMessage.BrandNameAlreadyExists);
            }
            var brand = _mapper.Map<Brand>(dto);
            await _unitOfWork.BrandRepository.AddAsync(brand);
            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0) return new ErrorResult("Add failed");
            return new SuccessResult("Brand Added");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var brand = await _unitOfWork.BrandRepository.GetAsync(p => p.Id == id);

            if (brand is null)
                return new ErrorResult(ExceptionsMessage.BrandNotFound);

            _unitOfWork.BrandRepository.Remove(brand);

            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0)
                return new ErrorResult(ExceptionsMessage.DeleteFailed);

            return new SuccessResult(ExceptionsMessage.BrandDeleted);
        }

        public async Task<IDataResult<List<BrandGetDto>>> GetAllAsync()
        {
            var brands = await _unitOfWork.BrandRepository.GetAllAsync();
            var brandGetDtos =  _mapper.Map<List<BrandGetDto>>(brands);
            return new SuccessDataResult<List<BrandGetDto>>(brandGetDtos);

        }

        public async Task<IDataResult<BrandGetDto>> GetByIdAsync(int id)
        {
            var brand = await _unitOfWork.BrandRepository.GetAsync(p => p.Id == id);

            if (brand == null)
                return new ErrorDataResult<BrandGetDto>(null, "Brand tapılmadı");

            var brandGetDto = _mapper.Map<BrandGetDto>(brand);

            return new SuccessDataResult<BrandGetDto>(brandGetDto,"Butun Brendler  geldi");
        }

        public async Task<IResult> UpdateAsync(int id, BrandUpdateDto dto)
        {
           var brand = await _unitOfWork.BrandRepository.GetAsync(p=>p.Id==id);
            if (brand is null)
                return new ErrorResult(ExceptionsMessage.BrandNotFound);
            _mapper.Map(dto, brand);
            _unitOfWork.BrandRepository.Update(brand);
            await _unitOfWork.SaveChangesAsync();
            return new SuccessResult(ExceptionsMessage.BrandUpdated);
        }
    }
}
