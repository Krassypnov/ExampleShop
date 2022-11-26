using Catalog.DataAccess.Abstraction;
using Microsoft.EntityFrameworkCore;
using Models;


namespace Catalog.DataAccess.Repo
{
    public class ReserveRepository : IReserveRepository
    {
        private readonly CatalogContext dbContext;

        public ReserveRepository(CatalogContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddOrUpdate(IEnumerable<ReservedItem> items)
        {
            foreach (var item in items)
            {
                var dbItem = await dbContext.ReservedItems.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                if (dbItem is not null)
                {
                    var entry = dbContext.Entry(dbItem);
                    dbItem.Count += item.Count;
                    entry.Property(x => x.Count).IsModified = true;
                }
                else
                    await dbContext.ReservedItems.AddAsync(item);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReservedItem>> GetItems(int skip, int take)
            => await dbContext.ReservedItems
                                .AsNoTracking()
                                .Skip(skip)
                                .Take(take)
                                .ToListAsync();

        public async Task RemoveOrUpdate(IEnumerable<ReservedItem> items)
        {
            foreach (var item in items)
            {
                var dbItem = await dbContext.ReservedItems.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                if (dbItem is not null)
                {
                    var entry = dbContext.Entry(dbItem);
                    dbItem.Count -= item.Count;
                    if (dbItem.Count == 0)
                        dbContext.ReservedItems.Remove(dbItem);
                    else
                        entry.Property(x => x.Count).IsModified = true;
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
