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
    public class ContactsController : Controller
    {
        private readonly CRMContext _context;

        public ContactsController(CRMContext context)
        {
            _context = context;
        }

        // GET: Contacts
        [Authorize]
        public async Task<IActionResult> Index(string filter)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.userId = user.Id;
            List<Company> companiesList = _context.Company.ToList();
            string[] companies = new string[companiesList[companiesList.Count - 1].Id + 1];
            var i = 1;
            foreach (var item in companiesList)
            {
                i = item.Id;
                companies[i] = Convert.ToString(item.Name);
            }
            ViewBag.data = companies;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[usersList[usersList.Count - 1].Id + 1];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Login;
            }
            ViewBag.data2 = users;
            var qry = _context.Contact.AsNoTracking().OrderBy(p => p.Id).AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                qry = qry.Where(p => p.Surname.Contains(filter));
            }
            var model = await qry.ToListAsync();
            ViewBag.filter = filter;
            return View(model);
        }

        // GET: Contacts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }
            List<Company> companiesList = _context.Company.ToList();
            string[] companies = new string[companiesList[companiesList.Count - 1].Id + 1];
            var i = 1;
            foreach (var item in companiesList)
            {
                i = item.Id;
                companies[i] = item.Name;
            }
            ViewBag.data = companies;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[usersList[usersList.Count - 1].Id + 1];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Login;
            }
            ViewBag.data2 = users;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.userId = user.Id;
            return View(contact);
        }

        // GET: Contacts/Create
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

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.IsDeleted = 0;
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        // GET: Contacts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact.FindAsync(id);
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            if (contact == null)
            {
                return NotFound();
            }
            List<Company> companiesList = _context.Company.ToList();
            ViewBag.data = companiesList;
            if (user.Id != contact.UserId)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            return View(contact);
        }

        // GET: Contacts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            if (contact == null)
            {
                return NotFound();
            }
            if (user.Id != contact.UserId)
            {
                return NotFound();
            }
            List<Company> companiesList = _context.Company.ToList();
            string[] companies = new string[companiesList[companiesList.Count - 1].Id + 1];
            var i = 1;
            foreach (var item in companiesList)
            {
                i = item.Id;
                companies[i] = item.Name;
            }
            ViewBag.data = companies;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[usersList[usersList.Count - 1].Id + 1];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Login;
            }
            ViewBag.data2 = users;
            ViewBag.userId = user.Id;
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contact.FindAsync(id);
            contact.IsDeleted = 1;
            //_context.Contact.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.Id == id);
        }
    }
}
