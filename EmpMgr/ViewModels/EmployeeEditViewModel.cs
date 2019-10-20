using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmpMgr.ViewModels
{
    public class EmployeeEditViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessage = "Polje Ime je obavezno.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Polje Prezime je obavezno")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Polje Datum rođenja")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [MaxLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Remote("ValidateEmailAddressForEmployeeEdit", "Employee", AdditionalFields = "Id")]
        public string Email { get; set; }
    }
}
