﻿using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Controllers
{
    public class FansController : Controller
    {
        private readonly SportsDbContext _context;

        public FansController(SportsDbContext context)
        {
            _context = context;
        }

        // GET: Fans
        public async Task<IActionResult> Index(int? ID)
        {
            var viewModel = new SportClubViewModel
            {
                Fans = await _context.Fans
              .Include(i => i.Subscriptions)
              .AsNoTracking()
              .OrderBy(i => i.LastName)
              .ToListAsync()
            };
            if (ID != null)
            {
                ViewData["ClubID"] = ID;
                var clubIds = _context.Subscriptions.Where(f => f.FanId == ID).Select(f => f.SportClubId).ToList();
                viewModel.SportClubs = await _context.SportClubs.Where(s => clubIds.Contains(s.Id)).ToListAsync();

            }
            return View(viewModel);
        }

        // GET: Fans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans.FindAsync(id);
            if (fan == null)
            {
                return NotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (id != fan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FanExists(fan.Id))
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
            return View(fan);
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fans == null)
            {
                return Problem("Entity set 'SportsDbContext.Fans'  is null.");
            }
            var fan = await _context.Fans.FindAsync(id);
            if (fan != null)
            {
                _context.Fans.Remove(fan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditSubscriptions(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }
            var clubs = await _context.SportClubs.ToListAsync();
            var fan = await _context.Fans.Include(i => i.Subscriptions).FirstOrDefaultAsync(f => f.ID == ID);

            var viewModel = new FanSubscriptionViewModel
            {
                Fan = fan,
                Subscriptions = clubs.Select(sport => new SportClubSubscriptionViewModel
                {
                    SportClubId = sport.Id,
                    Title = sport.Title,
                    IsMember = fan.Subscriptions?.Any(sub => sub.SportClubId == sport.Id) ?? false
                })
            };
            return View(viewModel);
        }


        private bool FanExists(int id)
        {
            return _context.Fans.Any(e => e.Id == id);
        }
    }

  


}
