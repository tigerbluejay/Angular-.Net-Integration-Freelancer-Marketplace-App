using API.Interfaces;

namespace API.Data;

public class UnitOfWork(DataContext context, IUserRepository userRepository,
    IMessageRepository messageRepository, IProposalRepository proposalRepository,
    IPortfolioItemRepository portfolioItemRepository, IProjectRepository projectRepository
    ) : IUnitOfWork
{
    public IUserRepository UserRepository => userRepository;
    public IMessageRepository MessageRepository => messageRepository;
    public IProposalRepository ProposalRepository => proposalRepository;
    public IPortfolioItemRepository PortfolioItemRepository => portfolioItemRepository;
    public IProjectRepository ProjectRepository => projectRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}

