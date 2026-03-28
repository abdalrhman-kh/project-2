using Microsoft.EntityFrameworkCore;
using project_2.Data;

namespace project_2.Models.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        DataDbContext db;
        DbSet<T> set;
        public Repository(DataDbContext db)
        {
            this.db = db;
            this.set = db.Set<T>();
        }
        public void Add(T entity)
        {
            entity.CreateDate = DateTime.Now;
            set.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var data = Get(id);
            data.DeleteDate = DateTime.Now;
            set.Remove(data);
            db.SaveChanges();
        }

        public void Edit(T entity)
        {
            var oldEntity = Get(entity.Id);
            entity.CreateDate = oldEntity.CreateDate;
            entity.DeleteDate = oldEntity.DeleteDate;
            entity.EditDate = DateTime.Now;
            set.Update(entity);
            db.SaveChanges();
        }

        public T Get(int id)
        {
            return set.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<T> GetAll()
        {
            return set.Where(x => !x.IsDeleted).ToList();
        }
    }
}
