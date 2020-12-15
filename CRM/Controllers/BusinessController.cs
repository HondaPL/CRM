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
    public class BusinessController : Controller
    {
        private readonly CRMContext _context;

        public BusinessController(CRMContext context)
        {
            _context = context;
        }

        // GET: Business
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Business.ToListAsync());
        }

        // GET: Business/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Business
                .FirstOrDefaultAsync(m => m.Id == id);
            //var user = await _context.User.ToListAsync(m => m.IdRole == role.Id);
            var company = await _context.Company.Where(m => m.BusinessId == business.Id).ToListAsync();
            if (business == null)
            {
                return NotFound();
            }
            ViewBag.Message = business.Name + " Companies";
            return View(company);
        }

        //// GET: Business/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Business/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name")] Business business)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(business);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(business);
        //}

        //// GET: Business/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var business = await _context.Business.FindAsync(id);
        //    if (business == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(business);
        //}

        //// POST: Business/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Business business)
        //{
        //    if (id != business.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(business);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BusinessExists(business.Id))
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
        //    return View(business);
        //}

        //// GET: Business/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var business = await _context.Business
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (business == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(business);
        //}

        //// POST: Business/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var business = await _context.Business.FindAsync(id);
        //    _context.Business.Remove(business);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool BusinessExists(int id)
        //{
        //    return _context.Business.Any(e => e.Id == id);
        //}
    }
}
