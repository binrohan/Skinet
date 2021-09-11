using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers {
    
    public class ProductsController : BaseApiController {
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandsRepo;
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IMapper _mapper;
        public ProductsController (IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductBrand> productBrandsRepo, 
            IGenericRepository<ProductType> productTypeRepo,
            IMapper mapper) {
            _productsRepo = productsRepo;
            _productBrandsRepo = productBrandsRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts () 
        {
            var spec = new ProductWithTypesAndBrandsSpecification();

            var products = await _productsRepo.ListAsync(spec);

            // Manual mapping
            // return Ok(
            //     products.Select(p => new ProductToReturnDto{
            //         Id = p.Id,
            //         Name = p.Name,
            //         Description = p.Description,
            //         PictureUrl = p.PictureUrl,
            //         Price = p.Price,
            //         ProductBrand = p.ProductBrand.Name,
            //         ProductType = p.ProductType.Name
            //     }).ToList()
            // ) ;

            return Ok(
                _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products)
            );
        }

        [HttpGet ("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct (int id) 
        {
            var spec = new ProductWithTypesAndBrandsSpecification(id);

            var product = await _productsRepo.GetEntityWithSpec(spec);

            if(product == null)
                return NotFound(new ApiResponse(404));
            
            // Manual Mapping
            // return new ProductToReturnDto
            // {
            //   Id = product.Id,
            //   Name = product.Name,
            //   Description = product.Description,
            //   PictureUrl = product.PictureUrl,
            //   Price = product.Price,
            //   ProductBrand = product.ProductBrand.Name,
            //   ProductType = product.ProductType.Name
            // };

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet ("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands () 
        {
            var productBrands = await _productBrandsRepo.ListAllAsync ();

            return Ok (productBrands);
        }

        [HttpGet ("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductType () 
        {
            var productTypes = await _productTypeRepo.ListAllAsync ();

            return Ok (productTypes);
        }
    }
}