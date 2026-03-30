using AutoMapper;
using Business.Abstract;
using Business.Dtos.Product;
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using Entities.Concrete;
using Entities.Enum;

public class ProductManager : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IDataResult<List<ProductGetDto>>> GetAllAsync()
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync(
            p => !p.IsDeleted,
            "Brand",
            "Category"
        );

        var productIds = products.Select(p => p.Id).ToList();

        var medias = await _unitOfWork.MediaRepository.GetAllAsync(
            m => m.OwnerType == MediaOwnerType.Product
                 && m.OwnerId.HasValue
                 && productIds.Contains(m.OwnerId.Value)
                 && !m.IsDeleted
        );

        var dtos = products.Select(p => new ProductGetDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            ImageUrls = medias
                .Where(m => m.OwnerId == p.Id)
                .OrderByDescending(m => m.IsMain)
                .Select(m => m.Url)
                .ToList()
        }).ToList();

        return new SuccessDataResult<List<ProductGetDto>>(dtos);
    }

    public async Task<IDataResult<List<ProductGetDto>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var products = await _unitOfWork.ProductRepository
            .GetAllWithPaginationAsync(pageNumber, pageSize, null, "Brand", "Category", "ProductImages");

        var dtos = _mapper.Map<List<ProductGetDto>>(products);
        return new SuccessDataResult<List<ProductGetDto>>(dtos);
    }
    public async Task<IDataResult<ProductGetDto>> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository.GetAsync(
            p => p.Id == id && !p.IsDeleted,
            "Brand",
            "Category"
        );

        if (product == null)
            return new ErrorDataResult<ProductGetDto>(null, "Məhsul tapılmadı");

        var medias = await _unitOfWork.MediaRepository.GetAllAsync(
            m => m.OwnerType == MediaOwnerType.Product
                 && m.OwnerId == id
                 && !m.IsDeleted
        );

        var dto = new ProductGetDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            BrandId = product.BrandId,
            BrandName = product.Brand?.Name,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            ImageUrls = medias
                .OrderByDescending(m => m.IsMain)
                .Select(m => m.Url)
                .ToList()
        };

        return new SuccessDataResult<ProductGetDto>(dto);
    }

    public async Task<IResult> AddAsync(ProductCreateDto dto)
    {
        // FK check (Brand)
        var brand = await _unitOfWork.BrandRepository.GetAsync(b => b.Id == dto.BrandId);
        if (brand == null) return new ErrorResult("Brand tapılmadı");

        // FK check (Category)
        var category = await _unitOfWork.CategoryRepository.GetAsync(c => c.Id == dto.CategoryId);
        if (category == null) return new ErrorResult("Category tapılmadı");

        // Duplicate check (MVP): Name + BrandId
        var exists = await _unitOfWork.ProductRepository
            .IsExistAsync(p => p.Name == dto.Name && p.BrandId == dto.BrandId);

        if (exists) return new ErrorResult("Bu adda product artıq var");
        var product = _mapper.Map<Product>(dto);
        product.RowVersion = null; // Və ya bazada avtomatik yaranması üçün boş buraxın
        product.IsDeleted = false; // Mütləq false set et
        await _unitOfWork.ProductRepository.AddAsync(product);

        var saved = await _unitOfWork.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult("Product əlavə olundu")
            : new ErrorResult("Product əlavə olunmadı");
    }

    public async Task<IResult> UpdateAsync(int id, ProductUpdateDto dto)
    {
        var product = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == id);
        if (product == null) return new ErrorResult("Product tapılmadı");

        var brand = await _unitOfWork.BrandRepository.GetAsync(b => b.Id == dto.BrandId);
        if (brand == null) return new ErrorResult("Brand tapılmadı");

        var category = await _unitOfWork.CategoryRepository.GetAsync(c => c.Id == dto.CategoryId);
        if (category == null) return new ErrorResult("Category tapılmadı");

        var exists = await _unitOfWork.ProductRepository
            .IsExistAsync(p => p.Name == dto.Name && p.BrandId == dto.BrandId && p.Id != id);

        if (exists) return new ErrorResult("Bu adda başqa product var");

        _mapper.Map(dto, product);
        _unitOfWork.ProductRepository.Update(product);

        var saved = await _unitOfWork.SaveChangesAsync();
        return saved > 0
            ? new SuccessResult("Product yeniləndi")
            : new ErrorResult("Product yenilənmədi");
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == id);
        if (product == null) return new ErrorResult("Product tapılmadı");

        _unitOfWork.ProductRepository.Remove(product);
        var saved = await _unitOfWork.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult("Product silindi")
            : new ErrorResult("Product silinmədi");
    }
}