using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //public class ProductsController(IGenericRepository<Product> repo) : BaseAPIController
    public class ProductsController(IUnitOfWork unit) : BaseAPIController
    {

        //Pour avoir la liste des produits
        //[FromQuery] dit à ASP.NET Core :
        //Va lire la query string de l’URL (?pageIndex=2&pageSize=6&brands=angular,react)
        //Trouve les noms correspondants (pageIndex, pageSize, brands)
        //Remplis automatiquement les propriétés de ta classe ProductSpecParams avec ces valeurs.


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
            [FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductSpecification(specParams);
            //return await CreatePageResult(repo, spec, specParams.PageIndex, specParams.PageSize);
            return await CreatePageResult(unit.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);
        }

        //pour récupérer un produit unique
        [HttpGet("{id:int}")] // api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            //var product = await repo.GetByIdAsync(id);
            var product = await unit.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            return product;

        }

        //Inserer un produit dans la bd
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            /* repo.Add(product);
            if (await repo.SaveAllAsync())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }*/
            unit.Repository<Product>().Add(product);
            if (await unit.Complete())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Problem creating product");
        }

        //Modifier un produit
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {

            if (product.Id != id || !ProductExists(id))
                return BadRequest("Cannot update this product");

            /* repo.Update(product);
            
            if (await repo.SaveAllAsync())
            {
                return NoContent();
            } */

            unit.Repository<Product>().Update(product);
            if (await unit.Complete())
            {
                return NoContent();
            }


            return BadRequest("Problem updating product");
        }

        //éfacer un produit
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            /* var product = await repo.GetByIdAsync(id);

            if (product == null) return NotFound();

            repo.Remove(product);
            if (await repo.SaveAllAsync())
            {
                return NoContent();
            } */

            var product = await  unit.Repository<Product>().GetByIdAsync(id);

            if (product == null) return NotFound();

            unit.Repository<Product>().Remove(product);
            if (await unit.Complete())
            {
                return NoContent();
            }
            return BadRequest("Problem deleting product");
        }


        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            //TODO: Implement method
            //return Ok(await repo.GetBrandsAsync());

            var spec = new BrandListSpecification();
            //return Ok(await repo.ListAsync(spec));
            return Ok(await unit.Repository<Product>().ListAsync(spec));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            //TODO: Implement method
            //return Ok(await repo.GetTypesAsync());

            var spec = new TypeListSpecification();
            // return Ok(await repo.ListAsync(spec));
            return Ok(await unit.Repository<Product>().ListAsync(spec));
        }

        private bool ProductExists(int id)
        {
            //return repo.Exists(id);
            return unit.Repository<Product>().Exists(id);
        }
    }
}
