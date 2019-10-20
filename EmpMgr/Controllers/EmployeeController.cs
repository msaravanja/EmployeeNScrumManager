using System;
using System.Linq;
using System.Threading.Tasks;
using EmpMgr.Data;
using EmpMgr.Models;
using EmpMgr.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmpMgr.Controllers
{
    [Authorize(Roles = "administrator")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public EmployeeController(ApplicationDbContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var listOfEmployees = await _context.Employees.ToListAsync().ConfigureAwait(false);
            return View(listOfEmployees);
        }

        // Get: Employee/Create
        public IActionResult Create()
        {
            return View(new EmployeeAddViewModel() { DateOfBirth = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,DateOfBirth,Email")] EmployeeAddViewModel employee)
        {
            if (!ModelState.IsValid) return View(employee);

            var employeeToAdd = new Employee()
            {
                UserName = employee.Email,
                NormalizedUserName = employee.Email.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Email = employee.Email,
                NormalizedEmail = employee.Email.ToUpper()
            };

            var createUser = await _userManager.CreateAsync(employeeToAdd, "P4$sw0rd").ConfigureAwait(false);

            if (!createUser.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            await _userManager.AddToRoleAsync(employeeToAdd, "employee").ConfigureAwait(false);

            return RedirectToAction(nameof(Index));
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var employeeToEdit = await _context.Employees.FindAsync(id).ConfigureAwait(false);
            if (employeeToEdit == null)
            {
                return View();
            }

            var employeeToEditVM = new EmployeeEditViewModel()
            {
                Id = employeeToEdit.Id,
                FirstName = employeeToEdit.FirstName,
                LastName = employeeToEdit.LastName,
                Email = employeeToEdit.Email,
                DateOfBirth = employeeToEdit.DateOfBirth
            };

            return View(employeeToEditVM);
        }

        // POST: DailyScrumTableModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,DateOfBirth,Email")] EmployeeEditViewModel employee)
        {
            if (id != employee.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var employeeToEdit = await _userManager.FindByIdAsync(employee.Id).ConfigureAwait(false);

                    if (employeeToEdit == null)
                        return View(employee);

                    employeeToEdit.FirstName = employee.FirstName;
                    employeeToEdit.LastName = employee.LastName;
                    employeeToEdit.Email = employee.Email;
                    employeeToEdit.NormalizedEmail = employee.Email.ToUpper();
                    employeeToEdit.DateOfBirth = employee.DateOfBirth;

                    await _userManager.UpdateAsync(employeeToEdit).ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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

            return View(employee);
        }
        
        // GET: Employee/Delete/5
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var employee = await _context.Employees.FindAsync(id).ConfigureAwait(false);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id).ConfigureAwait(false);

            if (currentUser != null)
            {
                var removeFromCurrentRole = await _userManager.RemoveFromRoleAsync(currentUser, "employee").ConfigureAwait(false);
                if (removeFromCurrentRole.Succeeded)
                {
                    await _userManager.AddToRoleAsync(currentUser, "administrator").ConfigureAwait(false);
                }
            }

            var listOfEmployees = await _context.Employees.ToListAsync().ConfigureAwait(false);

            return View("Index", listOfEmployees);
        }

        public async Task<IActionResult> DemoteToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id).ConfigureAwait(false);

            if (currentUser != null)
            {
                var removeFromCurrentRole = await _userManager.RemoveFromRoleAsync(currentUser, "administrator").ConfigureAwait(false);
                if (removeFromCurrentRole.Succeeded)
                {
                    await _userManager.AddToRoleAsync(currentUser, "employee").ConfigureAwait(false);
                }
            }

            var listOfEmployees = await _context.Employees.ToListAsync().ConfigureAwait(false);

            return View("Index", listOfEmployees);
        }

        public async Task<IActionResult> ValidateEmailAddress(string email)
        {
            var userWithEmail = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            return Json(userWithEmail == null ?
                "true" : string.Format("Adresa {0} je zauzeta.", email));
        }

        public async Task<IActionResult> ValidateEmailAddressForEmployeeEdit(string email, string Id)
        {
            var userWithEmail = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            var userWithId = await _userManager.FindByIdAsync(Id).ConfigureAwait(false);
            var emailIsTaken = true;

            if (userWithEmail != null)
            {
                emailIsTaken = userWithId.Email == email;
            }

            return Json(emailIsTaken ? "true" : string.Format("Adresa {0} je zauzeta.", email));
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}