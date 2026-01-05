using Microsoft.EntityFrameworkCore;
using Doctor.Appointment.Domain.Entities;

namespace Doctor.Appointment.Data.Context
{
    public class DoctorAppointmentContext : DbContext
    {
        public DoctorAppointmentContext(DbContextOptions<DoctorAppointmentContext> dbco) : base(dbco) { }

        public DbSet<AppointmentEntity> Appointment { get; set; }
        public DbSet<DoctorEntity> Doctors { get; set; }
        public DbSet<PatientEntity> Patients { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserEntity configuration
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Patient)
                    .WithMany()
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Doctor)
                    .WithMany()
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // RoleEntity configuration
            modelBuilder.Entity<RoleEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // UserRoleEntity configuration
            modelBuilder.Entity<UserRoleEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });

            // Seed roles
            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity { Id = 1, RoleName = "Guest", Description = "Guest user with limited access" },
                new RoleEntity { Id = 2, RoleName = "Patient", Description = "Patient user" },
                new RoleEntity { Id = 3, RoleName = "Doctor", Description = "Doctor user" },
                new RoleEntity { Id = 4, RoleName = "Admin", Description = "Administrator with full access" }
            );
        }
    }
}
