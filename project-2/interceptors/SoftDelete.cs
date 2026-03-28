using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using project_2.Models;

namespace project_2.interceptors
{
    public class SoftDelete : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var dbcontext = eventData.Context;
            if(dbcontext == null)
            {
                return new InterceptionResult<int>();
            }
            foreach(var entry in dbcontext.ChangeTracker.Entries<BaseEntity>())
            {
                if(entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeleteDate = DateTime.Now;
                }
            }
            return base.SavingChanges(eventData, result);
        }
    }
}
