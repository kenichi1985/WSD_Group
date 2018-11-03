﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;
using WebHotel.Models.RoomAvailabilityViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;


namespace WebHotel.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize (Roles ="Customers")]
        // GET: Rooms
        public async Task<IActionResult> Index(RoomAvailability searchRoom)
        {   
            var rooms = (IQueryable<Room>)_context.Room;
            ViewData["GreatherThan"] = null;
            ViewData["Passed"] = null;


            if (searchRoom.CheckOut.Date < searchRoom.CheckIn.Date){

                ViewData["GreatherThan"] = "The check out date earlier than check in date";
                return View(searchRoom);
            }

            if(searchRoom.CheckIn.Date < DateTime.Today && searchRoom.CheckIn.Date != DateTime.MinValue.Date)
            {
                ViewData["Passed"] = "The check in date already passed";
                return View(searchRoom);
            }

            if (searchRoom.BedCount != 0)
            {

                var numOfRoom = new SqliteParameter("numOfRoom", searchRoom.BedCount);
                var checkInDate = new SqliteParameter("checkInDate", searchRoom.CheckIn);
                var checkOutDate = new SqliteParameter("checkOutDate", searchRoom.CheckOut);


                string query = $"SELECT * FROM Room r WHERE BedCount = @numOfRoom " +
                 $"AND NOT EXISTS (SELECT 1 FROM Booking b WHERE b.RoomID = r.ID " +
                 $"AND (@checkInDate BETWEEN b.CheckIn AND b.CheckOut " +
                 $"OR @checkOutDate BETWEEN b.CheckIn AND b.CheckOut " +
                 $"OR (@checkInDate <= b.CheckIn AND @checkOutDate >= b.CheckOut)))";


                ViewBag.Result = await _context.Room.FromSql(query, numOfRoom, checkInDate, checkOutDate).Select(r => new Room { ID = r.ID, BedCount = r.BedCount, Level = r.Level, Price = r.Price }).ToListAsync();
                //var result = await _context.Room.FromSql(query, numOfRoom, checkInDate, checkOutDate).ToListAsync();

                //ViewBag.Result = await _context.Room.FromSql(query, numOfRoom, checkInDate, checkOutDate).ToListAsync();

                return View(searchRoom);
                //return View(result);

            }

            return View(searchRoom);
        }



        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }
        [Authorize]
        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Level,BedCount,Price")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }
        [Authorize]
        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Level,BedCount,Price")] Room room)
        {
            if (id != room.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.ID))
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
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.SingleOrDefaultAsync(m => m.ID == id);
            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.ID == id);
        }
    }
}
