using API.DTOs;
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

        public async Task<List<ProposalWithProjectCombinedDTO>> GetProposalsWithProjectsByFreelancerIdAsync(int freelancerId)
        {
            return await _context.Proposals
                .Where(p => p.FreelancerUserId == freelancerId)
                .Select(p => new ProposalWithProjectCombinedDTO
                {
                    Proposal = new ProposalDTO
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        Bid = p.Bid,
                        ProjectId = p.ProjectId,
                        FreelancerUserId = p.FreelancerUserId,
                        ClientUserId = p.ClientUserId,
                        FreelancerUsername = p.Freelancer.KnownAs,
                        ClientUsername = p.Client.KnownAs,
                        IsAccepted = p.IsAccepted,
                        PhotoUrl = p.Photo != null ? p.Photo.Url : null
                    },
                    Project = new ProjectBrowseDTO
                    {
                        Id = p.Project.Id,
                        Title = p.Project.Title,
                        Description = p.Project.Description,
                        IsAssigned = p.Project.IsAssigned,
                        SkillNames = p.Project.Skills.Select(s => s.Name).ToList(),
                        ClientUserId = p.Project.ClientUserId,
                        ClientUserName = p.Project.Client.KnownAs,
                        PhotoUrl = p.Project.Photo != null ? p.Project.Photo.Url : null,
                        ClientPhotoUrl = p.Project.Client.Photo.Url
                    }
                })
                .ToListAsync();
        }
    }
}