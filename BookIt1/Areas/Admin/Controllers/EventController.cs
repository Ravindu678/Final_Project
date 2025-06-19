

using System.Diagnostics;
using BookIt1.Areas.Admin.Models;
using BookIt1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;



namespace BookIt1.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class EventController : Controller
    {
        private readonly Data.BookIt1DbContext _context;

        public EventController(BookIt1DbContext context)
        {
            _context = context;
        }

        // GET: Admin/Event/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }



        // POST: Admin/Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload logic
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Define the path to save the uploaded file  
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists  
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to the server  
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }

                    // Save the relative file path to the database  
                    model.ImagePath = "/images/" + uniqueFileName;
                }

                // Automatically set AvailableSeats to TotalSeats if not provided
                model.AvailableTickets = model.TotalTickets;

                // Save event to the database
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model); // If model is invalid, return to the form with validation errors
        }


        // GET: Admin/Event/Edit/5  
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        // POST: Admin/Event/Edit/5  
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,EventDate,TotalTickets,Location,Category,Price,ImagePath")] Event eventItem, IFormFile ImageFile)
        {
            if (id != eventItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing event from the DB
                    var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (existingEvent == null)
                    {
                        return NotFound();
                    }

                    // Adjust AvailableSeats according to the new TotalSeats while preserving booked seats
                    int bookedSeats = existingEvent.TotalTickets - existingEvent.AvailableTickets;
                    eventItem.AvailableTickets = eventItem.TotalTickets - bookedSeats;

                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.Id))
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
            return View(eventItem);
        }*/

        //new code
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,EventDate,TotalTickets,Location,Category,Price")] Event eventItem, IFormFile ImageFile)
        {
            if (id != eventItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (existingEvent == null)
                    {
                        return NotFound();
                    }

                    // Handle image upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        eventItem.ImagePath = "/uploads/" + uniqueFileName;
                    }
                    else
                    {
                        // Keep old image path if new one not uploaded
                        eventItem.ImagePath = existingEvent.ImagePath;
                    }

                    // Adjust AvailableSeats based on new TotalTickets
                    int bookedSeats = existingEvent.TotalTickets - existingEvent.AvailableTickets;
                    eventItem.AvailableTickets = eventItem.TotalTickets - bookedSeats;

                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.Id))
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
            return View(eventItem);
        }


        //new code


        // GET: Admin/Event/Delete/5  
        public async Task<IActionResult> Delete(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }
        public async Task<IActionResult> DeleteView(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }
        // POST: Admin/Event/Delete/5  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // Fix: Remove the duplicate EventExists method  
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }

}
