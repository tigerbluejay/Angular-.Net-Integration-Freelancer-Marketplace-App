using System.Security.Claims;
using API.Controllers;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace API.UnitTests.Controllers;

public class ProjectControllerTests
{
	[Fact]
	public async Task DeleteProject_ValidOwner_ReturnsNoContent()
	{
		// Arrange

		var projectRepository = new Mock<IProjectRepository>();
		var userRepository = new Mock<IUserRepository>();
		var mapper = new Mock<IMapper>();

		var unitOfWork = new Mock<IUnitOfWork>();

		unitOfWork
			.Setup(u => u.ProjectRepository)
			.Returns(projectRepository.Object);

		unitOfWork
			.Setup(u => u.UserRepository)
			.Returns(userRepository.Object);

		unitOfWork
			.Setup(u => u.Complete())
			.ReturnsAsync(true);

		var owner = new AppUser
		{
			UserName = "jose",
			KnownAs = "Jose"
		};

		var project = new Project
		{
			Id = 10,
			Client = owner
		};

		projectRepository
			.Setup(r => r.GetProjectByIdAsync(10))
			.ReturnsAsync(project);

		var controller = new ProjectController(
			unitOfWork.Object,
			mapper.Object,
			MockUserManager(),
			null!);

		var user = new ClaimsPrincipal(
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
						User = user
					}
			};

		// Act

		var result =
			await controller.DeleteProject(10);

		// Assert

		Assert.IsType<NoContentResult>(result);

		projectRepository.Verify(
			r => r.DeleteProject(project),
			Times.Once);

		unitOfWork.Verify(
			u => u.Complete(),
			Times.Once);
	}

	private static UserManager<AppUser> MockUserManager()
	{
		var store =
			new Mock<IUserStore<AppUser>>();

		return new Mock<UserManager<AppUser>>(
			store.Object,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null).Object;
	}
}