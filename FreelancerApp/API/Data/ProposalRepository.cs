using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Data
{
    public class ProposalRepository : IProposalRepository
    {
        private readonly DataContext _context;

        public ProposalRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Proposal> CreateProposalAsync(Proposal proposal)
        {
            await _context.Proposals.AddAsync(proposal);
            return proposal; // optionally return the proposal with the generated Id
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Proposal?> GetProposalByIdAsync(int id)
        {
            return await _context.Proposals
                .Include(p => p.Freelancer)
                .Include(p => p.Client)
                .Include(p => p.Project)
                .Include(p => p.Photo)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Add(Proposal proposal)
        {
            _context.Proposals.Add(proposal);
        }
    }
}