//using GoogleGson;
using SQLite;
using SQLite.Net.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        public SQLiteConnection _context { get; }

        public Repository()
        {
            string _ruta = FileSystem.AppDataDirectory + "/departament.db3";
            _context = new SQLiteConnection(_ruta);
            _context.CreateTable<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _context.Table<T>();
        }
        public virtual T Get(int Id)
        {
            return _context.Find<T>(Id);
        }
        public virtual void Delete(T entity)
        {
            _context.Delete(entity);
        }
        public virtual void Update(T entity)
        {
            _context.Update(entity);
        }
        public virtual void Insert(T entity)
        {
            _context.Insert(entity);

        }
    }
}
