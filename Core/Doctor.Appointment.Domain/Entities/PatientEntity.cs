using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Appointment.Domain.Entities
{
    public class PatientEntity
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public long IdNumber { get; set; }
        public string? Address { get; set; }
        [Phone]
        public long CellNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }

    }
}
