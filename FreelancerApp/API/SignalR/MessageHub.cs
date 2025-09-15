using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IUnitOfWork unitOfWork, IMapper mapper) : Hub
{
    public override async Task OnConnectedAsync()
    {
        try
        {
            var httpContext = Context.GetHttpContext();
            var conversationIdStr = httpContext?.Request.Query["conversationId"];
            if (Context.User == null || string.IsNullOrEmpty(conversationIdStr))
                throw new Exception("Cannot join group");

            if (!int.TryParse(conversationIdStr, out var conversationId))
                throw new Exception("Invalid conversation id");

            var groupName = $"conversation-{conversationId}";
            Console.WriteLine($"[MessageHub] User {Context.User.GetUsername()} joining group {groupName}");

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await unitOfWork.MessageRepository.GetProjectMessages(conversationId);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MessageHub] OnConnectedAsync error: {ex.Message}");
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(MessageCreateDTO dto)
    {
        try
        {
            var username = Context.User?.GetUsername() ?? throw new Exception("Could not get user");
            var recipient = await unitOfWork.UserRepository.GetUserByIdAsync(dto.RecipientId);
            if (username == recipient?.KnownAs)
                throw new HubException("Cannot message yourself");

            var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
                throw new HubException("Cannot send message at this time");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = dto.Content,
                ConversationId = dto.ConversationId
            };

            var groupName = $"conversation-{dto.ConversationId}";
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);

            if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
                message.DateRead = DateTime.UtcNow;

            unitOfWork.MessageRepository.AddProjectMessage(message);
            if (await unitOfWork.Complete())
            {
                Console.WriteLine($"[MessageHub] Sending NewMessage to group {groupName}");
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MessageHub] SendMessage exception: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
        var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };
        if (group == null) { group = new Group { Name = groupName }; unitOfWork.MessageRepository.AddGroup(group); }
        group.Connections.Add(connection);
        return await unitOfWork.Complete();
    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await unitOfWork.MessageRepository.GetConnection(Context.ConnectionId);
        if (connection != null) { unitOfWork.MessageRepository.RemoveConnection(connection); await unitOfWork.Complete(); }
    }
}