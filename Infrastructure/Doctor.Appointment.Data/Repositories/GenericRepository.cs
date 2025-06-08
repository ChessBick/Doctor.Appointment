using Doctor.Appointment.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Appointment.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly DoctorAppointmentContext _context;
        private readonly DbSet<T> _entity;

        public GenericRepository(DoctorAppointmentContext doctorAppointmentContext)
        {
            _context = doctorAppointmentContext;
            _entity = _context.Set<T>();
        }

        public async void Delete(long id)
        {
            var record = await _entity.FindAsync(id);
            _entity.Remove(entity: record);
        }

        public IEnumerable<T> GetAll()
        {
            return [.. _entity];
        }

        public T GetById(long id)
        {
            return _entity.Find(id);
        }

        public void Insert(T obj)
        {
            _entity.Add(obj);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(T obj)
        {
            _entity.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
    }
}
