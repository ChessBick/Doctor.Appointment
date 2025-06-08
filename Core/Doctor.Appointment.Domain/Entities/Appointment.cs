using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Appointment.Domain.Entities
{
    public class Appointment
    {
        public long Id { get; set; }
        public DateTime AppointmentTime { get; set; }
    }
}
