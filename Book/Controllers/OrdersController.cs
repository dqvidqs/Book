using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Auth;
using System.Security.Claims;

namespace BookAPI.Controllers
{
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly BookContext _context;

        public OrdersController(BookContext context)
        {
            _context = context;
        }

        [Authorize(Roles.WORKER, Roles.CUSTOMER)]
        // GET: api/myorders
        [Route("api/myorders")]
        [HttpGet]
        public IEnumerable<Book> GetMyOrders()
        {
            string id = getUserId();

            var mybooks = (from b in _context.Books
                           join o in _context.Orders
                           on b.id equals o.book.id
                           where o.user.id == Convert.ToInt32(id)
                           select b).ToArray();
            return mybooks;
        }
        [Authorize(Roles.WORKER, Roles.CUSTOMER)]
        // GET: api/myorders
        [Route("api/myorders")]
        [HttpPost]
        public IActionResult PostOrder([FromBody] PostOrderBook orderingBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string StringUserID = getUserId();
            int id = orderingBook.book_id;
            OrderBook order = new OrderBook();
            order.book = _context.Books.Find(id);
            id = Convert.ToInt32(StringUserID);
            order.user = _context.Users.Find(id);
            _context.Orders.Add(order);
            _context.SaveChanges();

            return CreatedAtAction("Get Orders", new { id = order.id }, order);
        }
        [NonAction]
        public string getUserId()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            return claims.ElementAt(1).Value;

        }
        [Route("api/myorders/{id}")]
        [HttpDelete]
        [Authorize(Roles.WORKER, Roles.CUSTOMER)]
        public IActionResult DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return Ok(order);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.id == id);
        }
    }
}