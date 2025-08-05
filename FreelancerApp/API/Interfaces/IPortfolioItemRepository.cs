using API.Entities;

namespace API.Interfaces;

public interface IPortfolioItemRepository
{
    Task<PortfolioItem?> GetPortfolioItemByIdAsync(int id);
    void DeletePortfolioItem(PortfolioItem item);
    Task<bool> SaveAllAsync(); // optional if you're using context.SaveChangesAsync() directly
    Task<PortfolioItem> CreateAsync(PortfolioItem item);
    Task<bool> UpdateAsync(PortfolioItem item);
}