using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

public class ProposalsController(IUnitOfWork unitOfWork, IPhotoService photoService,
IMapper mapper) : BaseApiController
{
    // POST: api/proposals
    [HttpPost]
    public async Task<ActionResult<ProposalDTO>> CreateProposal([FromForm] ProposalCreateDTO dto)
    {
        var freelancer = await unitOfWork.UserRepository.GetUserByIdAsync(dto.FreelancerUserId);
        if (freelancer == null) return NotFound("Freelancer not found");

        var client = await unitOfWork.UserRepository.GetUserByIdAsync(dto.ClientUserId);
        if (client == null) return NotFound("Client not found");

        var project = await unitOfWork.ProjectRepository.GetProjectByIdAsync(dto.ProjectId);
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

        unitOfWork.ProposalRepository.Add(proposal);
        if (await unitOfWork.Complete())
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
        var proposal = await unitOfWork.ProposalRepository.GetProposalByIdAsync(id);
        if (proposal == null) return NotFound("Proposal not found");

        var proposalDto = mapper.Map<ProposalDTO>(proposal);
        return Ok(proposalDto);
    }

    [HttpGet("proposals-with-projects/{freelancerId}")]
    public async Task<IActionResult> GetProposalsWithProjects(
        int freelancerId,
        [FromQuery] ProposalWithProjectParams propprojParams)
    {
        var result = await unitOfWork.ProposalRepository
            .GetProposalsWithProjectsByFreelancerIdAsync(freelancerId, propprojParams);

        Response.AddPaginationHeader(result);

        return Ok(new
        {
            result = result,
            pagination = new
            {
                currentPage = result.CurrentPage,
                itemsPerPage = result.PageSize,
                totalItems = result.TotalCount,
                totalPages = result.TotalPages
            }
        });
    }

    [HttpGet("proposals-with-projects-inbox/{clientId}")]
    public async Task<IActionResult> GetProposalsWithProjectsInbox(
        int clientId,
        [FromQuery] ProposalWithProjectParams propprojParams)
    {
        var result = await unitOfWork.ProposalRepository
            .GetProposalsWithProjectsInboxByClientIdAsync(clientId, propprojParams);

        Response.AddPaginationHeader(result);

        return Ok(new
        {
            result = result,
            pagination = new
            {
                currentPage = result.CurrentPage,
                itemsPerPage = result.PageSize,
                totalItems = result.TotalCount,
                totalPages = result.TotalPages
            }
        });
    }
}