using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public async Task<PagedList<ProposalWithProjectCombinedDTO>> GetProposalsWithProjectsByFreelancerIdAsync(
    int freelancerId, ProposalWithProjectParams propprojParams)
        {
            var query = _context.Proposals
                .Where(p => p.FreelancerUserId == freelancerId);

            // Filter by status
            if (!string.IsNullOrEmpty(propprojParams.Status))
            {
                switch (propprojParams.Status.ToLower())
                {
                    case "accepted":
                        query = query.Where(p => p.IsAccepted == true);
                        break;
                    case "rejected":
                        query = query.Where(p => p.IsAccepted == false);
                        break;
                    case "pending":
                        query = query.Where(p => p.IsAccepted == null);
                        break;
                }
            }

            var projected = query.Select(p => new ProposalWithProjectCombinedDTO
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
            });

            return await PagedList<ProposalWithProjectCombinedDTO>.CreateAsync(
                projected, propprojParams.PageNumber, propprojParams.PageSize);
        }

        public async Task<PagedList<ProposalWithProjectCombinedDTO>> GetProposalsWithProjectsInboxByClientIdAsync(
int clientId, ProposalWithProjectParams propprojParams)
        {
            var query = _context.Proposals
                .Where(p => p.ClientUserId == clientId);

            // Filter by status
            if (!string.IsNullOrEmpty(propprojParams.Status))
            {
                query = query.Where(p => p.IsAccepted == null);
            
            }
            
            var projected = query.Select(p => new ProposalWithProjectCombinedDTO
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
                    PhotoUrl = p.Photo != null ? p.Photo.Url : null,
                    FreelancerPhotoUrl = p.Freelancer.Photo != null ? p.Freelancer.Photo.Url : null

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
            });

            return await PagedList<ProposalWithProjectCombinedDTO>.CreateAsync(
                projected, propprojParams.PageNumber, propprojParams.PageSize);
        }
    }
}