using API.Entities;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IProposalRepository
    {
        Task<Proposal> CreateProposalAsync(Proposal proposal);
        Task<Proposal?> GetProposalByIdAsync(int id);
        void Add(Proposal proposal);
        Task<bool> SaveAllAsync();
    }
}