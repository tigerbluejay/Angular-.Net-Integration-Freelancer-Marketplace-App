using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PortfolioItemRepository(DataContext context, IMapper mapper) : IPortfolioItemRepository
{
    public async Task<PortfolioItem?> GetPortfolioItemByIdAsync(int id)
    {
        return await context.PortfolioItems
        .Include(p => p.User)
        .Include(p => p.Photo)
        .FirstOrDefaultAsync(p => p.Id == id);
    }

    public void DeletePortfolioItem(PortfolioItem item)
    {
        context.PortfolioItems.Remove(item);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PortfolioItem> CreateAsync(PortfolioItem item)
    {
        context.PortfolioItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }


    public async Task<bool> UpdateAsync(PortfolioItem item)
    {
        context.Entry(item).State = EntityState.Modified;
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PagedList<PortfolioItemDTO>> GetFreelancerPortfolioAsync(int freelancerUserId, PortfolioParams portfolioParams)
    {
        var query = context.PortfolioItems
            .Where(p => p.UserId == freelancerUserId); // FK to freelancer

        var projected = query.ProjectTo<PortfolioItemDTO>(mapper.ConfigurationProvider);
        return await PagedList<PortfolioItemDTO>.CreateAsync(projected, portfolioParams.PageNumber, portfolioParams.PageSize);
    }
}