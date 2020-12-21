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
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CRM.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly CRMContext _context;

        public CompaniesController(CRMContext context)
        {
            _context = context;
        }

        // GET: Companies
        [Authorize]
        public async Task<IActionResult> Index(DateTime? start, DateTime? end, string category, int[] selected, int page = 1, string sortExpression = "Id")
        {
            //var qry = _context.Company.AsNoTracking().OrderBy(p => p.Id);
            var qry = _context.Company.AsNoTracking().OrderBy(p => p.Id).AsQueryable().Where(p => p.IsDeleted == 0);
            var qry1 = _context.Business.AsNoTracking().OrderBy(p => p.Id).AsQueryable();
            //Console.WriteLine(start?.ToString("yyyy-MM-dd"));
            //Console.WriteLine(end?.ToString("yyyy-MM-dd"));
            //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.start = start?.ToString("yyyy-MM-dd");
            ViewBag.end = end?.ToString("yyyy-MM-dd");
            ViewBag.userId = user.Id;
            //Console.WriteLine(category);
            //Console.WriteLine(selected);
            for(int x = 0; x < selected.Length; x++)
            {
                //Console.WriteLine(selected[x]);
            }
            //DateTime dt = new DateTime();
            //if (start == dt)
            //{
            //    start = "";
            //}
            if (start != null)
            {
                qry = qry.Where(m => m.CreationDate >= start);
            }
            if  (end != null)
            {
                qry = qry.Where(m => m.CreationDate <= end);
            }
            //Console.WriteLine(_context.Company.Find(1).CreationDate);
            //if (start < _context.Company.Find(1).CreationDate)
            //{
            //    Console.WriteLine("yes");
            //} else
            //{
            //    Console.WriteLine("no");
            //}
            //if (!string.IsNullOrWhiteSpace(category))
            ViewBag.selected = null;
            if(selected.Length > 0)
            {
                ViewBag.selected = selected;
                //Console.WriteLine(category);
                //qry1 = qry1.Where(p => p.Name.Contains(category));
                //qry1 = qry1.Where(p => p.Id == Convert.ToInt32(category));
                //var q = _context.Business.Join(_context.Company, p => p.Id, pm => pm.BusinessId);
                //var query = from Company in _context.Company
                //            join Business in _context.Business
                //                on Convert.ToInt32(Company.BusinessId) equals Business.Id
                //            select new { Company};

                int[] data = new int[100];
                var q = 0;
                for (int x = 0; x < selected.Length; x++)
                {
                    //Console.WriteLine(selected[x]);
                    data[q] = selected[x];
                    //Console.WriteLine(number.Id);
                    q++;
                }
                if (start != null)
                {
                    if (end != null)
                    {
                        qry = _context.Company.Where(p => data.Contains(p.BusinessId) && p.CreationDate >= start && p.CreationDate <= end);
                    } else
                    {
                        qry = _context.Company.Where(p => data.Contains(p.BusinessId) && p.CreationDate >= start);

                    }
                } else
                {
                    if (end != null)
                    {
                        qry = _context.Company.Where(p => data.Contains(p.BusinessId) && p.CreationDate <= end);
                    }
                    else
                    {
                        qry = _context.Company.Where(p => data.Contains(p.BusinessId));

                    }
                }
                //qry = qry.Where(p => p.BusinessId == data[1]);

                //var k = 1;
                //var qry2 = qry.Where(p => p.BusinessId == data[k]);
                ////var qry3 = qry.Where(p => p.BusinessId == data[k]);
                //var qry3 = qry.Where(p => p.BusinessId == "1");
                ////IQueryable<CRM.Models.Company> union = qry.Where(p => p.BusinessId == data[k]);
                ////Console.WriteLine("-----");
                //for (k = 0; k < j; k++)
                //{
                //    Console.WriteLine(data[k]);
                //    qry2 = qry.Where(p => p.BusinessId.Contains(data[k]));
                //    foreach (var number in qry2)
                //    {
                //        Console.WriteLine(number.Id + " ? " + number.Name);
                //    }
                //    //Console.WriteLine("++++");
                //    qry3 = qry3.Concat(qry2);
                //    qry2 = null;
                //    foreach (var number in qry3)
                //    {
                //        Console.WriteLine(number.Id + "  ! " +  number.Name);
                //    }
                //    //Console.WriteLine("-----");
                //}
                ////qry = qry.Where(m => m.BusinessId.Contains(data));
                ////qry.Where(r => r.BusinessId.Contains(data));
                //Console.WriteLine("-?---");
                //foreach (var number in qry)
                //{
                //    Console.WriteLine(number.Id + " " + number.Name);
                //}
            } else
            {
                if (start != null)
                {
                    if (end != null)
                    {
                        qry = _context.Company.Where(p => p.CreationDate >= start && p.CreationDate <= end);
                    }
                    else
                    {
                        qry = _context.Company.Where(p => p.CreationDate >= start);

                    }
                }
                else
                {
                    if (end != null)
                    {
                        qry = _context.Company.Where(p => p.CreationDate <= end);
                    }
                }
            }

            var model = await PagingList.CreateAsync(qry, 3, page, sortExpression, "Id");
            model.RouteValue = new RouteValueDictionary {
                { "filter", category},
                { "start", start },
                { "end", end }
            };
            List<Business> businessesList = _context.Business.ToList();
            string[] businesses = new string[businessesList[businessesList.Count - 1].Id + 1];
            var i = 1;
            foreach (var item in businessesList)
            {
                i = item.Id;
                businesses[i] = item.Name;
            }
            ViewBag.data = businesses;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[usersList[usersList.Count - 1].Id + 1];
            var j = 1;
            foreach (var item in usersList)
            {
                //Console.WriteLine(j);
                j = item.Id;
                users[j] = item.Name;
            }
            ViewBag.data2 = users;
            return View(model);
        }

        //GET: Companies/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.message = user.Id;
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            ViewBag.userId = user.Id;
            List<Business> businessesList = _context.Business.ToList();
            string[] businesses = new string[businessesList[businessesList.Count - 1].Id + 1];
            var i = 1;
            foreach (var item in businessesList)
            {
                i = item.Id;
                businesses[i] = item.Name;
            }
            ViewBag.data = businesses;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[usersList[usersList.Count - 1].Id + 1];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Name;
            }
            ViewBag.data2 = users;
            return View(company);
        }

        // GET: Companies/Create
        [Authorize]
        public async Task<ActionResult> CreateAsync()
        {
            //var model = new CompanyAndBusiness
            //{
            //    Business = await _context.Business.ToListAsync(),
            //    Company = new Company(),
            //    User = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value)
            //};
            List<Business> businessesList = _context.Business.ToList();
            ViewBag.data = businessesList;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            ViewBag.message = user.Id;
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            //return View(model.Company);
            if (ModelState.IsValid)
            {
                try {
                company.IsDeleted = 0;
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                } catch (DbUpdateException)
                {
                    List<Business> businessesList = _context.Business.ToList();
                    ViewBag.data = businessesList;
                    var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
                    ViewBag.message = user.Id;
                    ModelState.AddModelError("", "NIP Is Taken");
                    return View(company);
                }
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            List<Business> businessesList = _context.Business.ToList();
            ViewBag.data = businessesList;
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            
            if (company == null)
            {
                return NotFound();
            }
            if (user.Id != company.UserId)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }
            List<Business> businessesList = _context.Business.ToList();
            ViewBag.data = businessesList;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch(DbUpdateException)
                {
                    ModelState.AddModelError("", "NIP is taken");
                    return View(company);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            if (user.Id != company.UserId)
            {
                return NotFound();
            }
            List<Business> businessesList = _context.Business.ToList();
            string[] businesses = new string[100];
            var i = 1;
            foreach (var item in businessesList)
            {
                i = item.Id;
                businesses[i] = item.Name;
            }
            ViewBag.data = businesses;
            List<User> usersList = _context.User.ToList();
            string[] users = new string[100];
            var j = 1;
            foreach (var item in usersList)
            {
                j = item.Id;
                users[j] = item.Name;
            }
            ViewBag.data2 = users;
            ViewBag.userId = user.Id;
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Company.FindAsync(id);

            /*var noteNumber = await _context.Note.CountAsync(m => m.CompanyId == company.Id);
            while (noteNumber > 0)
            {
                var noteToDelete = await _context.Note.FirstOrDefaultAsync(m => m.CompanyId == company.Id);
                _context.Note.Remove(noteToDelete);
                await _context.SaveChangesAsync();
                noteNumber--;
            }
            var contactNumber = await _context.Contact.CountAsync(m => m.CompanyId == company.Id);
            while (contactNumber > 0)
            {
                var contactToDelete = await _context.Contact.FirstOrDefaultAsync(m => m.CompanyId == company.Id);
                _context.Contact.Remove(contactToDelete);
                await _context.SaveChangesAsync();
                contactNumber--;
            }
            _context.Company.Remove(company);
            */
            company.IsDeleted = 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
