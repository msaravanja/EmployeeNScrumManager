using System;
using System.Collections.Generic;

namespace EmpMgr.ViewModels
{
    public class DailyScrumViewModel
    {
        public Guid Id { get; set; }
        public DateTime ScrumDay { get; set; }
        public string TodoToday { get; set; }
        public string DoneYesterday { get; set; }
        public string Impediments { get; set; }
        public DateTime DailyScrumMeetingArrival { get; set; }
        public bool AttendedDailyScrumMeeting { get; set; }
        public string EmployeeId { get; set; }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Employees { get; set; }
    }
}
