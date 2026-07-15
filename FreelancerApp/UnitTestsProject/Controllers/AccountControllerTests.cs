using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace API.UnitTests.Controllers;

public class AccountControllerTests
{
	[Fact]
	public async Task Register_ValidRequest_ReturnsUserDto()
	{
		// Arrange

		var options = new DbContextOptionsBuilder<DataContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString())
		.Options;

		var context = new DataContext(options);

		var mapper = new Mock<IMapper>();

		var tokenService = new Mock<ITokenService>();

		var userManager = MockUserManager();

		var dto = new RegisterDTO
		{
			Username = "Jose",
			Password = "Password123!",
			Role = "Developer"
		};

		var mappedUser = new AppUser
		{
			UserName = "jose",
			KnownAs = "Jose"
		};

		mapper
			.Setup(m => m.Map<AppUser>(dto))
			.Returns(mappedUser);

		Mock.Get(userManager)
			.Setup(u => u.CreateAsync(
				It.IsAny<AppUser>(),
				dto.Password))
			.ReturnsAsync(IdentityResult.Success);

		Mock.Get(userManager)
			.Setup(u => u.AddToRoleAsync(
				It.IsAny<AppUser>(),
				dto.Role))
			.ReturnsAsync(IdentityResult.Success);

		tokenService
			.Setup(t => t.CreateToken(It.IsAny<AppUser>()))
			.ReturnsAsync("fake-jwt-token");

		var controller = new AccountController(
			context,
			userManager,
			tokenService.Object,
			mapper.Object);

		// Act

		var result = await controller.Register(dto);

		// Assert

		var actionResult =
			Assert.IsType<ActionResult<UserDTO>>(result);

		var userDto =
			Assert.IsType<UserDTO>(actionResult.Value);

		Assert.Equal("jose", userDto.Username);

		Assert.Equal(
			"fake-jwt-token",
			userDto.Token);

		Mock.Get(userManager).Verify(
			u => u.CreateAsync(
				It.IsAny<AppUser>(),
				dto.Password),
			Times.Once);

		Mock.Get(userManager).Verify(
			u => u.AddToRoleAsync(
				It.IsAny<AppUser>(),
				dto.Role),
			Times.Once);

		tokenService.Verify(
			t => t.CreateToken(It.IsAny<AppUser>()),
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