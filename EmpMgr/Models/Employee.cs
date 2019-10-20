using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EmpMgr.Models
{
    public class Employee : IdentityUser
    {
        // Extending default IdentityUser
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<DailyScrumModel> DailyScrumModels { get; set; }
    }
}
