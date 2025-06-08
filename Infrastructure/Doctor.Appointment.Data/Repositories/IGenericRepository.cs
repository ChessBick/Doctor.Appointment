using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Appointment.Data.Repositories
{
    internal interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(long id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(long id);
        void Save();
    }
}
