namespace API.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IMessageRepository MessageRepository { get; }
    IProposalRepository ProposalRepository { get; }
    IPortfolioItemRepository PortfolioItemRepository { get; }
    IProjectRepository ProjectRepository { get; }

    Task<bool> Complete();
    bool HasChanges();

}