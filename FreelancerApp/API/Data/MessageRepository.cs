using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext context;
    private readonly IMapper mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public void AddProjectMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<ProjectConversation?> GetConversationByProjectAsync(int projectId)
    {
        return await context.ProjectConversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ProjectId == projectId);
    }

    public async Task<ProjectConversation?> GetConversationByProjectAsync(int projectId, int freelancerId)
    {
        return await context.ProjectConversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ProjectId == projectId && c.FreelancerId == freelancerId);
    }

    public async Task<IEnumerable<MessageDTO>> GetProjectMessages(int conversationId)
    {
        var messages = await context.Messages
            .Include(x => x.Sender).ThenInclude(x => x.Photo)
            .Include(x => x.Recipient).ThenInclude(x => x.Photo)
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages = messages
            .Where(m => m.DateRead == null)
            .ToList();

        if (unreadMessages.Any())
        {
            unreadMessages.ForEach(m => m.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        return mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public async Task<IEnumerable<ProjectConversationDTO>> GetConversationsForUserAsync(int userId)
    {
        var conversations = await context.ProjectConversations
            .Include(c => c.Project)
            .Include(c => c.Messages)
            .Include(c => c.Client).ThenInclude(u => u.Photo)
            .Include(c => c.Freelancer).ThenInclude(u => u.Photo)
            .Where(c => c.ClientId == userId || c.FreelancerId == userId)
            .ToListAsync();

        var conversationDTOs = conversations.Select(c =>
        {
            var lastMessage = c.Messages.OrderByDescending(m => m.MessageSent).FirstOrDefault();
            var unreadCount = c.Messages.Count(m => m.DateRead == null && m.RecipientId == userId);

            return new ProjectConversationDTO
            {
                ConversationId = c.Id,
                ProjectId = c.ProjectId,
                ProjectTitle = c.Project.Title, // assuming Project has a Title
                ClientId = c.ClientId,
                ClientUsername = c.Client.UserName,
                ClientPhotoUrl = c.Client.Photo.Url,
                FreelancerId = c.FreelancerId,
                FreelancerUsername = c.Freelancer.UserName,
                FreelancerPhotoUrl = c.Freelancer.Photo.Url,
                LastMessage = lastMessage?.Content ?? "",
                LastMessageSent = lastMessage?.MessageSent ?? DateTime.MinValue,
                UnreadCount = unreadCount
            };
        });

        return conversationDTOs;
    }

    public async Task<Message?> GetMessageByIdAsync(int id)
    {
        return await context.Messages
            .Include(m => m.Sender)
                .ThenInclude(s => s.Photo)
            .Include(m => m.Recipient)
                .ThenInclude(r => r.Photo)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MessageDTO>> GetProjectMessages(int conversationId, int currentUserId)
    {
        var messages = await context.Messages
            .Include(x => x.Sender).ThenInclude(x => x.Photo)
            .Include(x => x.Recipient).ThenInclude(x => x.Photo)
            .Where(m => m.ConversationId == conversationId &&
                        // Filter out messages deleted by current user
                        !((m.SenderId == currentUserId && m.SenderDeleted) ||
                          (m.RecipientId == currentUserId && m.RecipientDeleted)))
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages = messages
            .Where(m => m.DateRead == null && m.RecipientId == currentUserId)
            .ToList();

        if (unreadMessages.Any())
        {
            unreadMessages.ForEach(m => m.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        return mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }


    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);

    }


    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }

}