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
    public class RolesController : Controller
    {
        private readonly CRMContext _context;

        public RolesController(CRMContext context)
        {
            _context = context;
        }

        // GET: Roles
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Role.ToListAsync());
        }

        // GET: Roles/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Role
                .FirstOrDefaultAsync(m => m.Id == id);
            //var user = await _context.User.ToListAsync(m => m.IdRole == role.Id);
            var user = await _context.User.Where(m => m.RoleId == Convert.ToString(role.Id)).ToListAsync();
            var usera = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.userId = Convert.ToString(usera.Id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.Message = role.Name + "s";
            return View(user);
        }

        //// GET: Roles/Create
        //[Authorize]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Roles/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name")] Role role)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(role);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(role);
        //}

        //// GET: Roles/Edit/5
        //[Authorize]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var role = await _context.Role.FindAsync(id);
        //    if (role == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(role);
        //}

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Role role)
        //{
        //    if (id != role.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(role);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!RoleExists(role.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(role);
        //}

        //// GET: Roles/Delete/5
        //[Authorize]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var role = await _context.Role
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (role == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(role);
        //}

        //// POST: Roles/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var role = await _context.Role.FindAsync(id);
        //    _context.Role.Remove(role);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool RoleExists(int id)
        //{
        //    return _context.Role.Any(e => e.Id == id);
        //}
    }
}
