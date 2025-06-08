using Microsoft.EntityFrameworkCore;
using Doctor.Appointment.Domain.Entities;

namespace Doctor.Appointment.Data.Context
{
    public class DoctorAppointmentContext:DbContext
    {
        public DoctorAppointmentContext(DbContextOptions<DoctorAppointmentContext> dbco) : base(dbco) { }


        public DbSet<Domain.Entities.Appointment> Appointment { get; set; }
        public DbSet<Domain.Entities.Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }  

    }
}
