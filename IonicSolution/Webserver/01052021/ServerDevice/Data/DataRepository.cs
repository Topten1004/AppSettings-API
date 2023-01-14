using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerDevice.Data
{
   
        public class DataRepository<T> : IDataRepository<T> where T : class
        {
            private readonly ServerDeviceContext _context;

            public DataRepository(ServerDeviceContext context)
            {
                _context = context;
            }

            public void Add(T entity)
            {
                _context.Set<T>().Add(entity);
            }

            public void Update(T entity)
            {
                _context.Set<T>().Update(entity);
            }

            public void Delete(T entity)
            {
                _context.Set<T>().Remove(entity);
            }

            public async Task<T> SaveAsync(T entity)
            {
                await _context.SaveChangesAsync();
                return entity;
            }
        }


}
