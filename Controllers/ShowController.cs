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
            if (id == null || _context.Shows == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .FirstOrDefaultAsync(m => m.ShowId == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // GET: Show/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Show/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShowId,RoomId,FilmId,ShowDate,Price,Status,Slot")] Show show)
        {
            if (ModelState.IsValid)
            {
                _context.Add(show);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(show);
        }

        // GET: Show/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shows == null)
            {
                return NotFound();
            }

            var show = await _context.Shows.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }
            return View(show);
        }

        // POST: Show/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShowId,RoomId,FilmId,ShowDate,Price,Status,Slot")] Show show)
        {
            if (id != show.ShowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(show);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.ShowId))
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
            return View(show);
        }

        // GET: Show/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shows == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .FirstOrDefaultAsync(m => m.ShowId == id);
            if (show == null)
            {
                return NotFound();
            }

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
