using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PRN_ASG3.Models;

namespace PRN_ASG3.Controllers
{
    public class BookingsController : Controller
    {
        private readonly CinemaContext _context;

        public BookingsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(int id)
        {
            Show show = new CinemaContext().Shows.Find(id);
            Boolean[,] roomMap = new Boolean[(int)show.Room.NumberRows, (int)show.Room.NumberCols];
            List<Booking> bookings = await _context.Bookings.Where(b => b.ShowId == id).ToListAsync();

            foreach(Booking booking in bookings)
            {
                string[] seats = booking.SeatStatus.Split('-');
                foreach (string seat in seats)
                {
                    if (ParseSeat(seat, out int a, out int b))
                    {
                        roomMap[a, b] = true;
                    }
                }
            }

            ViewBag.RoomMap = roomMap;
            ViewBag.ShowId = id;

            return _context.Bookings != null ? 
                          View(bookings) :
                          Problem("Entity set 'CinemaContext.Bookings'  is null.");
        }

        public static bool ParseSeat(string input, out int a, out int b)
        {
            a = 0;
            b = 0;

            string[] parts = input.Split('.');
            if (parts.Length != 2)
            {
                return false; // Not in the expected format "a.b"
            }

            if (int.TryParse(parts[0], out a) && int.TryParse(parts[1], out b))
            {
                return true; // Successfully parsed both "a" and "b" as integers
            }

            return false; // Unable to parse one or both values as integers
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create(int id)
        {
            Show show = new CinemaContext().Shows.Find(id);
            Boolean[,] roomMap = new Boolean[(int)show.Room.NumberRows, (int)show.Room.NumberCols];
            List<Booking> bookings = new CinemaContext().Bookings.Where(b => b.ShowId == id).ToList();

            foreach (Booking booking in bookings)
            {
                string[] seats = booking.SeatStatus.Split('-');
                foreach (string seat in seats)
                {
                    
                    if (ParseSeat(seat, out int a, out int b))
                    {
                        roomMap[a, b] = true;
                    }
                }
            }

            ViewBag.RoomMap = roomMap;
            ViewBag.ShowId = id;
            ViewBag.Price = show.Price;

            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            int showId = Convert.ToInt32(Request.Form["ShowId"]);
            string name = Request.Form["Name"];
            decimal amount = Convert.ToDecimal(Request.Form["Amount"]);

            string[] selectedSeats = Request.Form["Seats"];
            string seatStatus = "";

            foreach (string seatValue in selectedSeats)
            {
                // If the value is not null or empty, it means the checkbox is selected
                if (!string.IsNullOrEmpty(seatValue))
                {
                    seatStatus += seatValue + "-";
                }
            }


            Booking newBooking = new Booking
            {
                ShowId = showId, // Set the ShowId property
                Name = name, // Set the Name property
                SeatStatus = seatStatus, // Set the SeatStatus property
                Amount = amount // Set the Amount property
            };

            // Get an instance of your DbContext
            using (var dbContext = new CinemaContext())
            {
                // Add the new Booking entity to the DbSet
                dbContext.Bookings.Add(newBooking);

                // Save the changes to the database
                dbContext.SaveChanges();
            }


            return RedirectToAction("Index", "Bookings", new { id = showId });
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,ShowId,Name,SeatStatus,Amount")] Booking booking)
        {
            if (id != booking.BookingId)
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
                    if (!BookingExists(booking.BookingId))
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
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(m => m.BookingId == id);
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
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'CinemaContext.Bookings'  is null.");
            }
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
          return (_context.Bookings?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }
    }
}
