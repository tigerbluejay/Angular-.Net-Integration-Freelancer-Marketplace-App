using System.Security.Claims;
using API.Controllers;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace API.UnitTests.Controllers;

public class PhotosControllerTests
{
	[Fact]
	public async Task DeleteUserPhoto_PhotoExists_ReturnsOk()
	{
		// Arrange

		var unitOfWork = new Mock<IUnitOfWork>();

		var userRepository = new Mock<IUserRepository>();

		var projectRepository = new Mock<IProjectRepository>();

		var portfolioRepository = new Mock<IPortfolioItemRepository>();

		unitOfWork
			.Setup(x => x.UserRepository)
			.Returns(userRepository.Object);

		unitOfWork
			.Setup(x => x.ProjectRepository)
			.Returns(projectRepository.Object);

		unitOfWork
			.Setup(x => x.PortfolioItemRepository)
			.Returns(portfolioRepository.Object);

		unitOfWork
			.Setup(x => x.Complete())
			.ReturnsAsync(true);

		var photoService = new Mock<IPhotoService>();

		photoService
			.Setup(x => x.DeletePhotoAsync("cloudinary-id"))
			.ReturnsAsync(new DeletionResult());

		var mapper = new Mock<IMapper>();

		var user = new AppUser
		{
			UserName = "jose",
			KnownAs = "Jose",

			Photo = new Photo
			{
				Id = 5,
				PublicId = "cloudinary-id",
				Url = "photo.jpg"
			},

			PhotoId = 5
		};

		userRepository
			.Setup(x => x.GetUserByUsernameAsync("jose"))
			.ReturnsAsync(user);

		var controller = new PhotosController(
			unitOfWork.Object,
			photoService.Object,
			mapper.Object);

		var claimsPrincipal =
			new ClaimsPrincipal(
				new ClaimsIdentity(
					new[]
					{
						new Claim(
							ClaimTypes.NameIdentifier,
							"jose")
					}));

		controller.ControllerContext =
			new ControllerContext
			{
				HttpContext =
					new DefaultHttpContext
					{
						User = claimsPrincipal
					}
			};

		// Act

		var result =
			await controller.DeleteUserPhoto();

		// Assert

		var ok =
			Assert.IsType<OkObjectResult>(result);

		Assert.Equal(
			"User photo deleted",
			ok.Value);

		photoService.Verify(
			x => x.DeletePhotoAsync("cloudinary-id"),
			Times.Once);

		unitOfWork.Verify(
			x => x.Complete(),
			Times.Once);

		Assert.Null(user.Photo);

		Assert.Null(user.PhotoId);
	}
}