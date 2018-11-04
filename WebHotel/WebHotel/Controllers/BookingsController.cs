﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;
using WebHotel.Models.StatisticsViewModels;

namespace WebHotel.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: Bookings
        public async Task<IActionResult> Index(String sortOrder)
        {

            if (String.IsNullOrEmpty(sortOrder))
            {
                
                sortOrder = "surname_asc";
            }

            // Prepare the query for getting the entire list of Booking.
            // Convert the data type from DbSet<Booking> to IQueryable<Booking>
            var bookings = (IQueryable<Booking>)_context.Booking;

          
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

            var loggedInCustomer = bookings.Where(b => b.CustomerEmail.Equals(_email));
            
       
            var applicationDbContext = loggedInCustomer.Include(b => b.TheCustomer).Include(b => b.TheRoom);

            if (_email == "admin@wsh.com")
            {
                 applicationDbContext = _context.Booking.Include(b => b.TheCustomer).Include(b => b.TheRoom);

            }

            return View(await applicationDbContext.ToListAsync());
        }
       
      
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> Statistics(CalcPostcodeStats postcodeStats, CalcRoomIDStats roomIDStats, CalcBedCountStats bedCountStats) {
            var roomGroup = _context.Booking.GroupBy(b => b.RoomID);
            // booking Group for roomIDStats
            //ViewBag for storing RoomID statistics 
            ViewBag.RoomIDStats = await roomGroup.Select(b => new CalcRoomIDStats { RoomID = b.Key, NoOfBookings = b.Count() }).ToListAsync();


            var postcodeGroups = _context.Customer.GroupBy(b => b.Postcode); // booking group for postcode statistic function 
            //ViewBag for storing postcode statistics  
            ViewBag.postcodeStats = await postcodeGroups.Select(g => new CalcPostcodeStats { PostcodeOfCustomer = g.Key, NoOfCustomer = g.Count() }).ToListAsync();


            // booking Group for roomIDStats
            var bookingsGroup = _context.Booking.Include(p => p.TheRoom).GroupBy(b => b.TheRoom.BedCount);
            //ViewBag for Room ID Stats 
            ViewBag.BedCountStats = await bookingsGroup.Select(b => new CalcBedCountStats { BedCount = b.Key, NoOfBookings = b.Count() }).ToListAsync();



            return View();
        }
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            ViewBag.Success = null;
            ViewBag.Message = "";

            if (ModelState.IsValid)
            {

                var selectedRoom = new SqliteParameter("RoomID", booking.RoomID);
                var checkInDate = new SqliteParameter("checkInDate", booking.CheckIn);
                var checkOutDate = new SqliteParameter("checkOutDate", booking.CheckOut);

                var theCustomer = await _context.Customer.FindAsync(booking.CustomerEmail);

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
                        
                        RoomID = booking.RoomID,
                        CustomerEmail = booking.CustomerEmail,
                        CheckIn = booking.CheckIn,
                        CheckOut = booking.CheckOut
                    };


                    //calculate the total

                    int totalDay = (booking.CheckOut - booking.CheckIn).Days;

                    var theRoom = await _context.Room.FindAsync(booking.RoomID);

                    var totalCost = theRoom.Price * totalDay;

                    newBooking.Cost = totalCost;
                    _context.Add(newBooking);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = "The room is successfully booked.";
                    ViewBag.Message2 = "Room Level: " + theRoom.Level;
                    ViewBag.Message3 = "Check in date: " + booking.CheckIn;
                    ViewBag.Message4 = "Check out date: " + booking.CheckOut;
                    ViewBag.Message5 = "Cost: $" + totalCost;
                    ViewBag.Success = true;
                    return View(booking);
                }

                else
                {
                    // for displaying confirmation

                    ViewBag.Message = "Sorry, the room is not available";

                    ViewBag.Success = false;
                    return View();
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
        [Authorize]
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
