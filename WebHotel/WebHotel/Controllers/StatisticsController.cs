using System;
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
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
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
      
       
    }
}
