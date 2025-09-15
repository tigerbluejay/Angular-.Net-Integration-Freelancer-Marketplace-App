using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/photos")]
public class PhotosController(IUnitOfWork unitOfWork,
IPhotoService photoService, IMapper mapper) : BaseApiController
{

    [HttpPost("user")]
    public async Task<ActionResult<PhotoDTO>> UploadUserPhoto(IFormFile file)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User not found");

        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);

        // Delete old if exists
        if (user.Photo?.PublicId != null)
            await photoService.DeletePhotoAsync(user.Photo.PublicId);

        user.Photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (await unitOfWork.Complete())
            return Ok(new PhotoDTO { Id = user.Photo.Id, Url = user.Photo.Url, PublicId = user.Photo.PublicId });

        return BadRequest("Problem saving photo");
    }

    [HttpPost("project/{projectId}")]
    public async Task<ActionResult<PhotoDTO>> UploadProjectPhoto(int projectId, IFormFile file)
    {
        var project = await unitOfWork.ProjectRepository.GetProjectByIdAsync(projectId);
        if (project == null) return NotFound("Project not found");

        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);

        // Delete old photo if exists
        if (project.Photo?.PublicId != null)
            await photoService.DeletePhotoAsync(project.Photo.PublicId);

        project.Photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (await unitOfWork.Complete())
            return Ok(new PhotoDTO { Id = project.Photo.Id, Url = project.Photo.Url, PublicId = project.Photo.PublicId });

        return BadRequest("Problem saving project photo");
    }

    [HttpPost("portfolio/{portfolioItemId}")]
    public async Task<ActionResult<PhotoDTO>> UploadPortfolioPhoto(int portfolioItemId, IFormFile file)
    {
        var portfolioItem = await unitOfWork.PortfolioItemRepository.GetPortfolioItemByIdAsync(portfolioItemId);
        if (portfolioItem == null) return NotFound("Portfolio item not found");

        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);

        if (portfolioItem.Photo?.PublicId != null)
            await photoService.DeletePhotoAsync(portfolioItem.Photo.PublicId);

        portfolioItem.Photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (await unitOfWork.Complete())
            return Ok(new PhotoDTO { Url = portfolioItem.Photo.Url, Id = portfolioItem.Photo.Id, PublicId = portfolioItem.Photo.PublicId });

        return BadRequest("Problem saving portfolio photo");
    }

    [HttpDelete("user")]
    public async Task<ActionResult> DeleteUserPhoto()
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return NotFound("User not found");

        if (user.Photo == null) return BadRequest("User does not have a photo to delete");

        if (user.Photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(user.Photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        // Remove photo navigation and id
        user.Photo = null;
        user.PhotoId = null;

        if (await unitOfWork.Complete())
            return Ok("User photo deleted");

        return BadRequest("Problem deleting user photo");
    }

    [HttpDelete("project/{projectId}")]
    public async Task<ActionResult> DeleteProjectPhoto(int projectId)
    {
        var project = await unitOfWork.ProjectRepository.GetProjectByIdAsync(projectId);
        if (project == null) return NotFound("Project not found");

        if (project.Photo == null) return BadRequest("Project does not have a photo to delete");

        if (project.Photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(project.Photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        project.Photo = null;
        project.PhotoId = null;

        if (await unitOfWork.Complete())
            return Ok("Project photo deleted");

        return BadRequest("Problem deleting project photo");
    }

    [HttpDelete("portfolio/{portfolioItemId}")]
    public async Task<ActionResult> DeletePortfolioPhoto(int portfolioItemId)
    {
        var portfolioItem = await unitOfWork.PortfolioItemRepository.GetPortfolioItemByIdAsync(portfolioItemId);
        if (portfolioItem == null) return NotFound("Portfolio item not found");

        if (portfolioItem.Photo == null) return BadRequest("Portfolio item does not have a photo to delete");

        if (portfolioItem.Photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(portfolioItem.Photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        portfolioItem.Photo = null;
        portfolioItem.PhotoId = null;

        if (await unitOfWork.Complete())
            return Ok("Portfolio item photo deleted");

        return BadRequest("Problem deleting portfolio photo");
    }
}