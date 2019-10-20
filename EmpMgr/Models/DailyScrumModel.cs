using System;
using System.ComponentModel.DataAnnotations;

namespace EmpMgr.Models
{
    public class DailyScrumModel
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime ScrumDay { get; set; }
        public string TodoToday { get; set; }
        public string DoneYesterday { get; set; }
        public string Impediments { get; set; }
        public bool AttendedDailyScrumMeeting { get; set; }
        //[DataType(DataType.Time)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime DailyScrumMeetingArrival { get; set; }
        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
