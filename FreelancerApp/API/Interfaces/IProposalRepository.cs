using API.DTOs;
using API.Entities;
using API.Helpers;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IProposalRepository
    {
        Task<Proposal> CreateProposalAsync(Proposal proposal);
        Task<Proposal?> GetProposalByIdAsync(int id);
        void Add(Proposal proposal);
        Task<bool> SaveAllAsync();
        Task<PagedList<ProposalWithProjectCombinedDTO>> GetProposalsWithProjectsByFreelancerIdAsync(int freelancerId, ProposalWithProjectParams propprojParams);
        Task<PagedList<ProposalWithProjectCombinedDTO>> GetProposalsWithProjectsInboxByClientIdAsync(int freelancerId, ProposalWithProjectParams propprojParams);

    }
}