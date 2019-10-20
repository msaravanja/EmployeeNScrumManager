using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpMgr.Data;
using EmpMgr.Models;
using EmpMgr.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace EmpMgr.Controllers
{
    public class DailyScrumModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyScrumModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyScrumTableModels
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DailyScrums.Include(d => d.Employee)
                .Where(x => x.ScrumDay.Date == DateTime.Today);
            return View(await applicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: DailyScrumTableModels/Details/5
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return View();
            }

            var dailyScrumTableModel = await _context.DailyScrums
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            return dailyScrumTableModel == null ? View() : View(dailyScrumTableModel);
        }

        // GET: DailyScrumTableModels/Details/5
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> GetDailyScrumForEmployee(Guid? id)
        {
            if (id == null)
            {
                return View();
            }
            var dailyScrumTableModel = await _context.DailyScrums
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.EmployeeId == id.ToString() && m.ScrumDay.Date == DateTime.Today.Date).ConfigureAwait(false);

            return dailyScrumTableModel == null ? View() : View(dailyScrumTableModel);
        }

        // GET: DailyScrumTableModels/Create
        [Authorize(Roles = "administrator")]
        public IActionResult Create()
        {
            // get all employees that are not assigned scrum for today
            var employees = _context.Employees
                .Where(emp => 
                    !_context.DailyScrums.Any(ds => ds.EmployeeId == emp.Id && ds.ScrumDay.Date == DateTime.Today))
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Email
                })
                .ToList();

            var employeesTip = new SelectListItem()
            {
                Value = null,
                Text = "--- Odaberite radnika ---"
            };
            employees.Insert(0, employeesTip);

            var dsvm = new DailyScrumViewModel() {
                Employees = new SelectList(employees, "Value", "Text"),
                DailyScrumMeetingArrival = DateTime.Today
            };

            return View(dsvm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Create([Bind("TodoToday,DoneYesterday,Impediments,DailyScrumMeetingArrival,EmployeeId")] DailyScrumViewModel dailyScrumModel)
        {
            if (ModelState.IsValid)
            {
                var dateTimeToCompare = DateTime.Parse("2000/01/01 08:45:00.000");
                if (dailyScrumModel.DailyScrumMeetingArrival.TimeOfDay < dateTimeToCompare.TimeOfDay)
                    dailyScrumModel.AttendedDailyScrumMeeting = true;

                var dsm = new DailyScrumModel
                {
                    Id = Guid.NewGuid(),
                    ScrumDay = DateTime.Today,
                    AttendedDailyScrumMeeting = dailyScrumModel.AttendedDailyScrumMeeting,
                    EmployeeId = dailyScrumModel.EmployeeId,
                    TodoToday = dailyScrumModel.TodoToday,
                    DoneYesterday = dailyScrumModel.DoneYesterday,
                    Impediments = dailyScrumModel.Impediments,
                    DailyScrumMeetingArrival = dailyScrumModel.DailyScrumMeetingArrival
                };

                _context.Add(dsm);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }

            return View(dailyScrumModel);
        }

        // GET: DailyScrumTableModels/Edit/5
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyScrumTableModel = await _context.DailyScrums.FindAsync(id).ConfigureAwait(false);
            if (dailyScrumTableModel == null)
            {
                return View();
            }

            return View(dailyScrumTableModel);
        }

        // POST: DailyScrumTableModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ScrumDay,TodoToday,DoneYesterday,Impediments,AttendedDailyScrumMeeting,DailyScrumMeetingArrival,EmployeeId")] DailyScrumModel dailyScrumModel)
        {
            if (id != dailyScrumModel.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dateTimeToCompare = DateTime.Parse("2000/01/01 08:45:00.000");
                    if (dailyScrumModel.DailyScrumMeetingArrival.TimeOfDay < dateTimeToCompare.TimeOfDay)
                        dailyScrumModel.AttendedDailyScrumMeeting = true;

                    _context.Update(dailyScrumModel);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyScrumTableModelExists(dailyScrumModel.Id))
                    {
                        return View();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(dailyScrumModel);
        }

        // GET: DailyScrumTableModels/Delete/5
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View();
            }

            var dailyScrumTableModel = await _context.DailyScrums
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            return dailyScrumTableModel == null ? View() : View(dailyScrumTableModel);
        }

        // POST: DailyScrumTableModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var dailyScrumTableModel = await _context.DailyScrums.FindAsync(id).ConfigureAwait(false);
            _context.DailyScrums.Remove(dailyScrumTableModel);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        private bool DailyScrumTableModelExists(Guid id)
        {
            return _context.DailyScrums.Any(e => e.Id == id);
        }
    }
}
