using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PRN_ASG3.Models;

namespace PRN_ASG3.Controllers
{
    public class ShowController : Controller
    {
        private readonly CinemaContext _context;

        public ShowController(CinemaContext context)
        {
            _context = context;
        }

        public void getList(List<Show> shows)
        {
            List<Room> rooms = _context.Rooms.ToList();
            List<Film> films = _context.Films.ToList();

            ViewData["JoinShow"] = from s in shows
                                   join r in rooms on s.RoomId equals r.RoomId into joinRoom
                                   from r in joinRoom.DefaultIfEmpty()
                                   join f in films on s.FilmId equals f.FilmId into joinFilm
                                   from f in joinFilm.DefaultIfEmpty()
                                   select new MutipleJoinClass
                                   {
                                       show = s,
                                       room = r,
                                       film = f
                                   };
            ViewBag.Rooms = rooms;
            ViewBag.Films = films;
        }

        // GET: Show
        [HttpGet]
        public IActionResult Index()
        {
            if (_context.Shows != null && _context.Rooms != null && _context.Films != null)
            {
                List<Show> shows = _context.Shows.ToList();
                getList(shows);

                return View(ViewData["JoinShow"]);
            }
            else
            {
                return Problem("Entity set 'CinemaContext.Shows' is null.");
            }
        }

        [HttpPost]
        public IActionResult Index(Show searchShow)
        {
            if (_context.Shows != null)
            {
                // Filter the list of shows based on the provided parameters.
                List<Show> shows = _context.Shows
                    .Where(show => show.ShowDate == searchShow.ShowDate &&
                           show.RoomId == searchShow.RoomId &&
                           show.FilmId == searchShow.FilmId)
                    .ToList();
                getList(shows);

                return View(ViewData["JoinShow"]);
            }
            else
            {
                return Problem("Entity set 'CinemaContext.Shows' is null.");
            }
        }

        // GET: Show/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .FirstOrDefaultAsync(s => s.ShowId == id);

            if (show == null)
            {
                return NotFound();
            }

            // Load the related Film information separately
            var film = await _context.Films.FirstOrDefaultAsync(f => f.FilmId == show.FilmId);
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == show.RoomId);

            // You can use ViewData to pass the Film data to the view
            ViewData["show"] = show;
            ViewData["film"] = film;
            ViewData["room"] = room;

            return View(show);
        }

        // GET: Show/Create
        public IActionResult Create()
        {
            DateTime currentDate = DateTime.Now.Date;

            List<Film> films = _context.Films.ToList();
            var slotsForCurrentDate = _context.Shows
                .Where(show => show.ShowDate.HasValue && 
                show.ShowDate.Value.Date == currentDate.Date && 
                show.RoomId == 1)
                .Select(show => show.Slot)
                .ToList();

            List<int> allSlots = Enumerable.Range(1, 9).ToList();
            List<int> emptySlots = allSlots.Where(slot => !slotsForCurrentDate.Contains(slot)).ToList();

            ViewBag.Films = films;
            ViewBag.Slot = emptySlots;
            return View();
        }

        // POST: Show/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Show newShow)
        {
            newShow.ShowDate = DateTime.Now;
            newShow.RoomId = 1;
            _context.Shows.Add(newShow);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Show/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shows == null)
            {
                return NotFound();
            }

            var editShow = await _context.Shows.FindAsync(id);
            if (editShow == null)
            {
                return NotFound();
            }

            DateTime currentDate = DateTime.Now.Date;

            List<Film> films = _context.Films.ToList();
            var slotsForCurrentDate = _context.Shows
                .Where(show => show.ShowDate.HasValue &&
                show.ShowDate.Value.Date == editShow.ShowDate &&
                show.Slot != editShow.Slot &&
                show.RoomId == 1)
                .Select(show => show.Slot)
                .ToList();

            List<int> allSlots = Enumerable.Range(1, 9).ToList();
            List<int> emptySlots = allSlots.Where(slot => !slotsForCurrentDate.Contains(slot)).ToList();

            ViewBag.Films = films;
            ViewBag.Slot = emptySlots;
            ViewBag.EditShow = editShow;

            return View(editShow);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShowId,FilmId,Price,Slot")] Show show)
        {
            if (id != show.ShowId)
            {
                return NotFound();
            }
            else
            {
                var existingShow = _context.Shows.Find(show.ShowId);
                if (existingShow != null)
                {
                    // Update the properties that can be modified
                    existingShow.FilmId = show.FilmId;
                    existingShow.Price = show.Price;
                    existingShow.Slot = show.Slot;

                    _context.Update(existingShow);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Show/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .FirstOrDefaultAsync(s => s.ShowId == id);

            if (show == null)
            {
                return NotFound();
            }

            // Load the related Film information separately
            var film = await _context.Films.FirstOrDefaultAsync(f => f.FilmId == show.FilmId);
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == show.RoomId);

            // You can use ViewData to pass the Film data to the view
            ViewData["show"] = show;
            ViewData["film"] = film;
            ViewData["room"] = room;

            return View(show);
        }

        // POST: Show/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shows == null)
            {
                return Problem("Entity set 'CinemaContext.Shows'  is null.");
            }
            var show = await _context.Shows.FindAsync(id);
            if (show != null)
            {
                _context.Shows.Remove(show);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowExists(int id)
        {
          return (_context.Shows?.Any(e => e.ShowId == id)).GetValueOrDefault();
        }
    }
}
