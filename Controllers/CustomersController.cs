using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NorthWindRestApi.Models;

namespace NorthWindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        NorthwindOriginalContext db;
        public CustomersController(NorthwindOriginalContext db) 
        { 
            this.db = db;
        }


        //Getterit
        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                var customer = db.Customers.ToList();
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetById(string id)
        {
            try
            {
                var customer = db.Customers.Find(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("companyname/{name}")]
        public ActionResult GetByName(string name)
        {
            try
            {
                var customer = db.Customers.Where(x => x.CompanyName.Contains(name));
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("country/{country}")]
        public ActionResult GetByCountry(string country)
        {
            try
            {
                var customer = db.Customers.Where(x => x.Country == country);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        //Posterit

        [HttpPost]
        public ActionResult AddNewCustomer([FromBody] Customer customer)
        {
            try
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return Ok($"Added new Customer {customer.CompanyName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }



        [HttpDelete("{id}")]
        public ActionResult RemoveCustomerAndItsOrders(string id, bool forceDelete = false)
            //Huom Force delete poistaa asiakkaan kaikki tilaukset ja tilausrivit
        {
            if (!forceDelete)
            {
                try
                {
                    var customer = db.Customers.Find(id);
                    if (customer is not null)
                    {
                        db.Customers.Remove(customer);
                        db.SaveChanges();
                        return Ok($"Customer {customer.CustomerId} removed");
                    }
                    return NotFound("Customer was not found!");

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException);
                }
            }
            else
            {
                try
                {
                    var customer = db.Customers.Find(id);
                    if (customer is not null)
                    {
                        var orders = db.Orders.Where(x => x.CustomerId == id).ToList();
                        if (orders.Count != 0)
                        {
                            var orderIds = orders.Select(order => order.OrderId).ToList();
                            var rivit = db.OrderDetails.Where(x => orderIds.Contains(x.OrderId)).ToList();
                            if (rivit.Count != 0)
                            {
                                foreach (var r in rivit)
                                {
                                    db.OrderDetails.Remove(r);
                                }
                                db.SaveChanges();
                            }

                            foreach (var i in orders)
                            {
                                db.Orders.Remove(i);
                            }
                            db.SaveChanges();
                        }
                        db.Customers.Remove(customer);
                        db.SaveChanges();
                        return Ok($"Customer {customer.CustomerId} removed");
                    }
                    return NotFound("Customer was not found!");

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException);
                }
            }
 
        }


        [HttpPut("{id}")]
        public ActionResult Update(string id, [FromBody] Customer cust)
        {
            try
            {
                var customer = db.Customers.Find(id);

                if (customer is not null)
                {
                    customer.CustomerId = cust.CustomerId;
                    customer.CompanyName = cust.CompanyName;
                    customer.ContactName = cust.ContactName;
                    customer.ContactTitle = cust.ContactTitle;
                    customer.Address = cust.Address;
                    customer.City = cust.City;
                    customer.Country = cust.Country;
                    customer.Region = cust.Region;
                    customer.PostalCode = cust.PostalCode;
                    customer.Phone = cust.Phone;
                    customer.Fax = cust.Fax;

                    db.Customers.Update(customer);
                    db.SaveChanges();
                    return Ok($"Customer {customer.CustomerId} changed");
                }
                return NotFound();

            }
            catch(Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }
    }
}
