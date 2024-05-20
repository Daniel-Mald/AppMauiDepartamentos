using SQLite;

namespace AppMauiDepartamentos.Repositories
{
    public interface IRepository<T> where T : class, new()
    {
        SQLiteConnection _context { get; }

        void Delete(T entity);
        T Get(int Id);
        IEnumerable<T> GetAll();
        void Insert(T entity);
        void Update(T entity);
    }
}