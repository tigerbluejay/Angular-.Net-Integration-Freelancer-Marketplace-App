using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

public class ProposalsController(IProposalRepository proposalRepository, IUserRepository userRepository,
    IProjectRepository projectRepository, IPhotoService photoService, IMapper mapper) : BaseApiController
{
    // POST: api/proposals
    [HttpPost]
    public async Task<ActionResult<ProposalDTO>> CreateProposal([FromForm] ProposalCreateDTO dto)
    {
        var freelancer = await userRepository.GetUserByIdAsync(dto.FreelancerUserId);
        if (freelancer == null) return NotFound("Freelancer not found");

        var client = await userRepository.GetUserByIdAsync(dto.ClientUserId);
        if (client == null) return NotFound("Client not found");

        var project = await projectRepository.GetProjectByIdAsync(dto.ProjectId);
        if (project == null) return NotFound("Project not found");

        var proposal = mapper.Map<Proposal>(dto);

        // Handle optional photo
        if (dto.PhotoFile != null)
        {
            var result = await photoService.AddPhotoAsync(dto.PhotoFile);
            if (result.Error != null) return BadRequest(result.Error.Message);

            proposal.Photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
        }

        proposalRepository.Add(proposal);
        if (await proposalRepository.SaveAllAsync())
        {
            var proposalDto = mapper.Map<ProposalDTO>(proposal);
            return CreatedAtAction(nameof(GetProposalById), new { id = proposal.Id }, proposalDto);
        }

        return BadRequest("Failed to create proposal");
    }

    // Optional: basic GET endpoint for CreatedAtAction
    [HttpGet("{id}")]
    public async Task<ActionResult<ProposalDTO>> GetProposalById(int id)
    {
        var proposal = await proposalRepository.GetProposalByIdAsync(id);
        if (proposal == null) return NotFound("Proposal not found");

        var proposalDto = mapper.Map<ProposalDTO>(proposal);
        return Ok(proposalDto);
    }
}