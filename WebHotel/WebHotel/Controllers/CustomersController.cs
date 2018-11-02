using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;
using WebHotel.Models.StatisticsViewModels;
namespace WebHotel.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .SingleOrDefaultAsync(m => m.Email == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
        [Authorize]
        // GET: Customers/Details/5
        public async Task<IActionResult> MyDetails()
        {
            // retrieve the logged-in user's email
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            var customer = await _context.Customer.FindAsync(_email);

            if (customer == null)
            {
                customer = new Customer { Email = _email };
                return View("~/Views/Customers/MyDetailsCreate.cshtml", customer);
            }
            else
            {
                return View("~/Views/Customers/MyDetailsUpdate.cshtml", customer);
            }
        }

        // POST: Purchases/MyDetailsCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsCreate([Bind("Email,Surname,GivenName,Postcode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();

                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }

        // POST: Purchases/MyDetailsCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsUpdate([Bind("Email,Surname,GivenName,Postcode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }
        [Authorize (Roles ="Administrators")]
        public async Task<IActionResult> CalcPostcodeStats()
        {
            var postcodeGroups = _context.Customer.GroupBy(b => b.Postcode);
            var postcodeStats = postcodeGroups.Select(g => new CalcPostcodeStats { PostcodeOfCustomer = g.Key, NoOfCustomer = g.Count() });
            return View(await postcodeStats.ToListAsync());
        }
        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Surname,GivenName,Postcode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.SingleOrDefaultAsync(m => m.Email == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email,Surname,GivenName,Postcode")] Customer customer)
        {
            if (id != customer.Email)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .SingleOrDefaultAsync(m => m.Email == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _context.Customer.SingleOrDefaultAsync(m => m.Email == id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(string id)
        {
            return _context.Customer.Any(e => e.Email == id);
        }
    }
}
