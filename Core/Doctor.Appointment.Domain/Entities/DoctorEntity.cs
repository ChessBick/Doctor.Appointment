using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Appointment.Domain.Entities
{
    public class DoctorEntity
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public long PracticeNumber { get; set; }
        public int Age { get; set; }
        public string? Qualification { get; set; }
    }
}
