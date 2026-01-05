using Microsoft.EntityFrameworkCore;
using Doctor.Appointment.Domain.Entities;

namespace Doctor.Appointment.Data.Context
{
    public class DoctorAppointmentContext:DbContext
    {
        public DoctorAppointmentContext(DbContextOptions<DoctorAppointmentContext> dbco) : base(dbco) { }


        public DbSet<Domain.Entities.AppointmentEntity> Appointment { get; set; }
        public DbSet<Domain.Entities.DoctorEntity> Doctors { get; set; }
        public DbSet<PatientEntity> Patients { get; set; }  

    }
}
