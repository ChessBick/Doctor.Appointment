using System;
using System.Collections.Generic;
using System.Text;

namespace Doctor.Appointment.Application.Services.IServices
{
    public interface IPasswordHasher
    {
        (string passwordHash, string passwordSalt) HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash, string passwordSalt);
    }
}
