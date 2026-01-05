using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doctor.Appointment.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<T?> GetByIdAsync(long id);
        T GetById(long id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(long id);
        void Save();
        Task SaveAsync();
    }
}
