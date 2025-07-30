using API.Entities;
using API.Interfaces;

namespace API.Data;

public class PortfolioItemRepository(DataContext context) : IPortfolioItemRepository
{
    public async Task<PortfolioItem?> GetPortfolioItemByIdAsync(int id)
    {
        return await context.PortfolioItems.FindAsync(id);
    }

    public void DeletePortfolioItem(PortfolioItem item)
    {
        context.PortfolioItems.Remove(item);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}