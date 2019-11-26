using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Auth;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
        [Route("api/orders")]
        [HttpGet]
        public IActionResult GetMyOrders()
        {
            var mybooks = (from b in _context.Books
                           join o in _context.Orders
                           on b.id equals o.book.id
                           where o.user.id == getUserId()
                           select b).ToArray();
            return Json(new
            {
                value = mybooks
            });
        }
        [Route("api/orders/{id}")]
        [HttpGet]
        public IActionResult GetMyBook([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mybook = (from b in _context.Books
                           join o in _context.Orders
                           on b.id equals o.book.id
                           where o.user.id == getUserId()
                           where b.id == id
                           select b).FirstOrDefault();

            if (mybook == null)
            {
                return NotFound();
            }
            return Json(new
            {
                value = mybook,
            });
            //return mybooks;
        }
        [Authorize(Roles.WORKER, Roles.CUSTOMER)]
        // GET: api/myorders
        [Route("api/orders")]
        [HttpPost]
        public IActionResult PostOrder([FromBody] PostOrderBook orderingBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int id = orderingBook.book_id;
            OrderBook order = new OrderBook();
            order.book = _context.Books.Find(id);
            id = getUserId();
            order.user = _context.Users.Find(id);
            if (order.book != null || order.user != null)
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            else
            {
                NotFound();
            }

            return Json(new
            {
                value = order.book
            });
        }
        [NonAction]
        public int getUserId()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            return Convert.ToInt32(claims.ElementAt(1).Value);

        }
        [Route("api/orders/{id}")]
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