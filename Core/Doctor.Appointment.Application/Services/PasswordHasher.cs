using Doctor.Appointment.Application.Services.IServices;
using System.Security.Cryptography;
using System.Text;

namespace Doctor.Appointment.Application.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int KeySize = 64;
        private const int Iterations = 350000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

        public (string passwordHash, string passwordSalt) HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(KeySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                HashAlgorithm,
                KeySize);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            var salt = Convert.FromBase64String(passwordSalt);
            var hash = Convert.FromBase64String(passwordHash);

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                HashAlgorithm,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(hashToCompare, hash);
        }
    }
}