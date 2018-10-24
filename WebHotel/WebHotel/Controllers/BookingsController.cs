using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;

namespace WebHotel.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(String sortOrder)
        {

            if (String.IsNullOrEmpty(sortOrder))
            {
                // When the Index page is loaded for the first time, the sortOrder is empty.
                // By default, the booking should be displayed in surname_asc.
                sortOrder = "surname_asc";
            }

            // Prepare the query for getting the entire list of purchase.
            // Convert the data type from DbSet<Purchase> to IQueryable<Purchase>
            var bookings = (IQueryable<Booking>)_context.Booking;

            // Sort the movies by specified order
            switch (sortOrder)
            {
                case "surname_asc":
                    bookings = bookings.OrderBy(b => b.TheCustomer.Surname);
                    break;
                case "surname_desc":
                    bookings = bookings.OrderByDescending(b => b.TheCustomer.Surname);
                    break;
                case "givenName_asc":
                    bookings = bookings.OrderBy(b => b.TheCustomer.GivenName);
                    break;
                case "givenName_desc":
                    bookings = bookings.OrderByDescending(b => b.TheCustomer.GivenName);
                    break;
                case "room_asc":
                    bookings = bookings.OrderBy(b => b.RoomID);
                    break;
                case "room_desc":
                    bookings = bookings.OrderByDescending(b => b.RoomID);
                    break;
                case "checkIn_asc":
                    bookings = bookings.OrderBy(b => b.CheckIn);
                    break;
                case "checkIn_desc":
                    bookings = bookings.OrderByDescending(b => b.CheckIn);
                    break;
                case "checkOut_asc":
                    bookings = bookings.OrderBy(b => b.CheckOut);
                    break;
                case "checkOut_desc":
                    bookings = bookings.OrderByDescending(b => b.CheckOut);
                    break;
                case "cost_asc":
                    bookings = bookings.OrderBy(b => b.Cost);
                    break;
                case "cost_desc":
                    bookings = bookings.OrderByDescending(b => b.Cost);
                    break;
            }

            // Deciding query string (sortOrder=xxx) to include in heading links for pizza name, qty of pizza and total respectively.
            // They specify the next display order if a heading link is clicked. 
            // Store them in ViewData dictionary to pass them to View.
            ViewData["SortSurname"] = sortOrder != "surname_asc" ? "surname_asc" : "surname_desc";
            ViewData["SortGivenName"] = sortOrder != "givenName_asc" ? "givenName_asc" : "givenName_desc";
            ViewData["SortRoom"] = sortOrder != "room_asc" ? "room_asc" : "room_desc";
            ViewData["SortCheckIn"] = sortOrder != "checkIn_asc" ? "checkIn_asc" : "checkIn_desc";
            ViewData["SortCheckOut"] = sortOrder != "checkOut_asc" ? "checkOut_asc" : "checkOut_desc";
            ViewData["SortCost"] = sortOrder != "cost_asc" ? "cost_asc" : "cost_desc";

            // retrieve the logged-in user's email
            string _email = User.FindFirst(ClaimTypes.Name).Value;

            var loginedCustomer = bookings.Where(b => b.CustomerEmail.Equals(_email));

            var applicationDbContext = loginedCustomer.Include(b => b.TheCustomer).Include(b => b.TheRoom);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            ViewData["CustomerEmail"] = _email;
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {

                var selectedRoom = new SqliteParameter("RoomID", booking.RoomID);
                var checkInDate = new SqliteParameter("checkInDate", booking.CheckIn);
                var checkOutDate = new SqliteParameter("checkOutDate", booking.CheckOut);


                string query = $"SELECT * FROM Booking " +
                    $"WHERE RoomID = @RoomID AND " +
                    $"(@checkInDate BETWEEN CheckIn AND CheckOut OR " +
                    $"@checkOutDate BETWEEN CheckIn AND CheckOut OR " +
                    $"(@checkInDate <= CheckIn AND @checkOutDate >= CheckOut))";

                var result = await _context.Booking.FromSql(query, selectedRoom, checkInDate, checkOutDate).ToListAsync();

                if (result.Count() == 0)
                {
                    var newBooking = new Booking
                    {
                        ID = booking.ID,
                        RoomID = booking.RoomID,
                        CustomerEmail = booking.CustomerEmail,
                        CheckIn = booking.CheckIn,
                        CheckOut = booking.CheckOut
                    };
                    _context.Add(newBooking);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                else
                {
                    return View(booking);
                }
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.ID))
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
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.ID == id);
        }
    }
}
