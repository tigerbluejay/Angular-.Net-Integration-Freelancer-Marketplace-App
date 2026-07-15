using System.Security.Claims;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace API.UnitTests.Controllers;

public class UsersControllerTests
{
	[Fact]
	public async Task UpdateUser_ValidRequest_ReturnsNoContent()
	{
		// Arrange

		var unitOfWork = new Mock<IUnitOfWork>();

		var userRepository = new Mock<IUserRepository>();

		unitOfWork
			.Setup(x => x.UserRepository)
			.Returns(userRepository.Object);

		unitOfWork
			.Setup(x => x.Complete())
			.ReturnsAsync(true);

		var mapper = new Mock<IMapper>();

		var dbOptions =
			new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

		var context = new DataContext(dbOptions);

		var existingUser = new AppUser
		{
			UserName = "jose",
			KnownAs = "Jose",
			FirstName = "Old Name"
		};

		context.Users.Add(existingUser);

		await context.SaveChangesAsync();

		userRepository
			.Setup(r => r.GetUserByUsernameAsync("jose"))
			.ReturnsAsync(existingUser);

		mapper
			.Setup(m =>
				m.Map(
					It.IsAny<MemberUpdateDTO>(),
					existingUser))
			.Callback<MemberUpdateDTO, AppUser>((dto, user) =>
			{
				user.FirstName = dto.FirstName;
			});

		var controller =
			new UsersController(
				unitOfWork.Object,
				mapper.Object,
				context);

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

		var dto = new MemberUpdateDTO
		{
			FirstName = "New Name"
		};

		// Act

		var result =
			await controller.UpdateUser(dto);

		// Assert

		Assert.IsType<NoContentResult>(result);

		mapper.Verify(
			m => m.Map(dto, existingUser),
			Times.Once);

		unitOfWork.Verify(
			u => u.Complete(),
			Times.Once);
	}
}