using API.Entities;
using API.DTOs;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddProjectMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessage(int id);
    Task<ProjectConversation?> GetConversationByProjectAsync(int projectId);
    Task<ProjectConversation?> GetConversationByProjectAsync(int projectId, int freelancerId);
    Task<IEnumerable<MessageDTO>> GetProjectMessages(int conversationId);
    Task<IEnumerable<ProjectConversationDTO>> GetConversationsForUserAsync(int userId);
    Task<Message?> GetMessageByIdAsync(int id);
    Task<bool> SaveAllAsync();
}