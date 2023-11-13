using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthWindRestApi.Models;

namespace NorthWindRestApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        NorthwindOriginalContext db;
        public ProductsController(NorthwindOriginalContext db)
        {
            this.db = db;
        }

        //Getterit
        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                var products = db.Products.ToList();
                if (!products.Any())
                {
                    return NotFound("Product table is empty");
                }

                List<Category> cate = db.Categories.ToList();

                foreach (var j in cate)
                {
                    j.Picture = null;
                    j.Products.Clear();
                }

                foreach (var i in products)
                {
                    i.Category = cate.Find(x => x.CategoryId == i.CategoryId);
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product is null)
                {
                    return NotFound($"No Product with id: {id} was found");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("productname/{name}")]
        public ActionResult GetByProductByName(string name, bool fullmatch = false)
        {
            if (!fullmatch)
            {
                try
                {
                    var product = db.Products.Where(x => x.ProductName.Contains(name));
                    if (!product.Any())
                    {
                        return NotFound("No matching products");
                    }
                    return Ok(product);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                try
                {
                    var product = db.Products.Where(x => x.ProductName == name);
                    if (!product.Any())
                    {
                        return NotFound("No matching product");
                    }
                    return Ok(product);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("Catergoryid/{id}")]
        public ActionResult GetByCatergoriaId(int id)
        {
            try
            {
                var product = db.Products.Where(x => x.CategoryId == id);
                if (!product.Any())
                {
                    return NotFound($"No Products in catergory id: {id} was found");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CatergoryName/{categorianame}")]
        public ActionResult GetByCatergoriaName(string categorianame)
        {
            try
            {
                //Huom ei voi hakea koko categoriaa objekteina, koska json parseri sekoaa kuvista mitä on kategoria taulussa
                var categoriaNames = db.Categories.Select(x => new { x.CategoryId, x.CategoryName }).ToList();

                if (!categoriaNames.Any())
                {
                    return NotFound("Catergoria table is empty!");
                }

                var categoriaId = categoriaNames.Find(x => x.CategoryName.ToLower() == categorianame.ToLower());
                if (categoriaId is null)
                {
                    return NotFound("No matching categoria!");
                }

                var product = db.Products.Where(x => x.CategoryId == categoriaId.CategoryId).ToList();

                if (!product.Any())
                {
                    return NotFound($"No Products in this catergoria was found");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //Posterit

        [HttpPost]
        public ActionResult AddNewProduct([FromBody] Product product)
        {
            try
            {
                //Pitäisikö estää jos on jo saman niminen tuote tietokannassa??
                db.Products.Add(product);
                db.SaveChanges();
                return Ok($"Added new Product {product.ProductName}");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }

        //Putterit

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Product product)
        {
            try
            {
                var modiProduct = db.Products.Find(id);

                if (modiProduct is not null)
                {

                    modiProduct.ProductName = product.ProductName;
                    modiProduct.SupplierId = product.SupplierId;
                    modiProduct.CategoryId = product.CategoryId;
                    modiProduct.QuantityPerUnit = product.QuantityPerUnit;
                    modiProduct.UnitPrice = product.UnitPrice;
                    modiProduct.UnitsInStock = product.UnitsInStock;
                    modiProduct.UnitsOnOrder = product.UnitsOnOrder;
                    modiProduct.ReorderLevel = product.ReorderLevel;
                    modiProduct.Discontinued = product.Discontinued;
                    modiProduct.ImageLink = product.ImageLink;

                    db.Products.Update(modiProduct);
                    db.SaveChanges();
                    return Ok($"Product {modiProduct.ProductId} Updated");
                }
                return NotFound($"Product with id: {id} was not found!");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Delete

        [HttpDelete("{id}")]
        public ActionResult RemoveProduct(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product is not null)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                    return Ok($"Product {product.ProductId} removed");
                }
                return NotFound("Product was not found!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }

    }
}
