using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Controllers
{
    public class NotesController : Controller
    {
        private readonly CRMContext _context;

        public NotesController(CRMContext context)
        {
            _context = context;
        }

        // GET: Notes
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.userId = Convert.ToString(user.Id);
            List<Company> companiesList = _context.Company.ToList();
            string[] companies = new string[100];
            var i = 1;
            foreach (var item in companiesList)
            {
                i = item.Id;
                companies[i] = Convert.ToString(item.Name);
            }
            ViewBag.data = companies;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[100];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Login;
            }
            ViewBag.data2 = users;
            return View(await _context.Note.ToListAsync());
        }

        // GET: Notes/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }
            List<Company> companiesList = _context.Company.ToList();
            string[] companies = new string[100];
            var i = 1;
            foreach (var item in companiesList)
            {
                i = item.Id;
                companies[i] = item.Name;
            }
            ViewBag.data = companies;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[100];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Login;
            }
            ViewBag.data2 = users;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.userId = Convert.ToString(user.Id);
            return View(note);
        }

        // GET: Notes/Create
        [Authorize]
        public async Task<IActionResult> CreateAsync(int? com)
        {
            ViewBag.id = com;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            var company = await _context.Company.FindAsync(com);
            if (company != null)
            {
                ViewBag.name = company.Name;
            }
            ViewBag.user = user.Id;
            List<Company> companiesList = _context.Company.ToList();
            ViewBag.data = companiesList;
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,CompanyId,UserId")] Note note)
        {
            if (ModelState.IsValid)
            {
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        // GET: Notes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.FindAsync(id);
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            if (note == null)
            {
                return NotFound();
            }
            List<Company> companiesList = _context.Company.ToList();
            ViewBag.data = companiesList;
            if (Convert.ToString(user.Id) != note.UserId)
            {
                return NotFound();
            }
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,CompanyId,UserId")] Note note)
        {
            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
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
            return View(note);
        }

        // GET: Notes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            var note = await _context.Note
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }
            if (Convert.ToString(user.Id) != note.UserId)
            {
                return NotFound();
            }
            List<Company> companiesList = _context.Company.ToList();
            string[] companies = new string[100];
            var i = 1;
            foreach (var item in companiesList)
            {
                i = item.Id;
                companies[i] = item.Name;
            }
            ViewBag.data = companies;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[100];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Login;
            }
            ViewBag.data2 = users;
            ViewBag.userId = Convert.ToString(user.Id);
            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Note.FindAsync(id);
            _context.Note.Remove(note);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Note.Any(e => e.Id == id);
        }
    }
}
