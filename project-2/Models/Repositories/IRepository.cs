namespace project_2.Models.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        void Add(T entity);
        void Edit(T entity);
        void Delete(int id);
        T Get(int id);
        List<T> GetAll();
    }
}
