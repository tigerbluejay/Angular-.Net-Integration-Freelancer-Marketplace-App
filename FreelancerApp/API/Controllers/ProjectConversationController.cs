using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectConversationController : ControllerBase
{
    private readonly IMessageRepository messageRepository;
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;

    public ProjectConversationController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    {
        this.messageRepository = messageRepository;
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    [HttpPost("{projectId}")]
    public async Task<ActionResult<MessageDTO>> SendMessage(int projectId, MessageCreateDTO dto)
    {
        var currentUser = await userRepository
            .GetUserByUsernameAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var conversation = await messageRepository.GetConversationByProjectAsync(projectId);
        if (conversation == null) return BadRequest("No conversation exists for this project.");

        // Optional safety check: ensure current user is part of the conversation
        if (conversation.ClientId != currentUser.Id && conversation.FreelancerId != currentUser.Id)
            return Forbid();

        var message = new Message
        {
            SenderId = currentUser.Id,
            RecipientId = dto.RecipientId,
            Content = dto.Content,
            ConversationId = conversation.Id
        };

        messageRepository.AddProjectMessage(message);

        if (await messageRepository.SaveAllAsync())
        {
            // Re-fetch the message with navigation properties (Sender, Recipient)
            var savedMessage = await messageRepository.GetMessageByIdAsync(message.Id);
            return Ok(mapper.Map<MessageDTO>(savedMessage));
        }

        return BadRequest("Failed to send message");
    }

    [HttpGet("{projectId}/{freelancerId}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages(int projectId, int freelancerId)
    {
        var conversation = await messageRepository.GetConversationByProjectAsync(projectId, freelancerId);
        if (conversation == null) return NotFound();

        var messages = await messageRepository.GetProjectMessages(conversation.Id);
        return Ok(messages);
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<IEnumerable<ProjectConversationDTO>>> GetConversations()
    {
        var currentUser = await userRepository
                .GetUserByUsernameAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (currentUser == null) return Unauthorized();

        var conversations = await messageRepository.GetConversationsForUserAsync(currentUser.Id);
        return Ok(conversations.OrderByDescending(c => c.LastMessageSent));
    }
}