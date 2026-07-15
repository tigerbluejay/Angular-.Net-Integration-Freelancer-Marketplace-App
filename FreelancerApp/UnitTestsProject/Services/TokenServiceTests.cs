using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace API.UnitTests.Services;

public class TokenServiceTests
{
	private readonly Mock<IConfiguration> _configuration;
	private readonly Mock<UserManager<AppUser>> _userManager;

	public TokenServiceTests()
	{
		_configuration = new Mock<IConfiguration>();

		var userStore = new Mock<IUserStore<AppUser>>();

		_userManager = new Mock<UserManager<AppUser>>(
			userStore.Object,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null);
	}

	[Fact]
	public async Task CreateToken_ReturnsJwt_WhenUserHasNoRoles()
	{
		// Arrange

		var tokenKey = new string('A', 64);

		_configuration
			.Setup(c => c["TokenKey"])
			.Returns(tokenKey);

		_userManager
			.Setup(u => u.GetRolesAsync(It.IsAny<AppUser>()))
			.ReturnsAsync(new List<string>());

		var service =
			new TokenService(
				_configuration.Object,
				_userManager.Object);

		var user = new AppUser
		{
			UserName = "jose",
			KnownAs = "Jose"
		};

		// Act

		var token = await service.CreateToken(user);

		// Assert

		Assert.False(string.IsNullOrWhiteSpace(token));

		var handler = new JwtSecurityTokenHandler();

		Assert.True(handler.CanReadToken(token));
	}

	[Fact]
	public async Task CreateToken_IncludesRoleClaims()
	{
		// Arrange

		var tokenKey = new string('A', 64);

		_configuration
			.Setup(c => c["TokenKey"])
			.Returns(tokenKey);

		_userManager
			.Setup(u => u.GetRolesAsync(It.IsAny<AppUser>()))
			.ReturnsAsync(new List<string>
			{
				"Admin",
				"Developer"
			});

		var service =
			new TokenService(
				_configuration.Object,
				_userManager.Object);

		var user = new AppUser
		{
			UserName = "jose",
			KnownAs = "Jose"
		};

		// Act

		var token = await service.CreateToken(user);

		// Assert

		var jwt =
			new JwtSecurityTokenHandler()
				.ReadJwtToken(token);

		var roles = jwt.Claims
			   .Where(c => c.Value == "Admin" ||
						   c.Value == "Developer")
			   .Select(c => c.Value)
			   .ToList();

		Assert.Contains("Admin", roles);
		Assert.Contains("Developer", roles);
	}

	[Fact]
	public async Task CreateToken_Throws_WhenTokenKeyTooShort()
	{
		// Arrange

		_configuration
			.Setup(c => c["TokenKey"])
			.Returns("abc");

		var service =
			new TokenService(
				_configuration.Object,
				_userManager.Object);

		var user = new AppUser
		{
			UserName = "jose",
			KnownAs = "Jose"
		};

		// Act / Assert

		var ex =
			await Assert.ThrowsAsync<Exception>(
				() => service.CreateToken(user));

		Assert.Equal(
			"Your tokenKey needs to be longer",
			ex.Message);
	}

	[Fact]
	public async Task CreateToken_Throws_WhenUsernameIsNull()
	{
		// Arrange

		var tokenKey = new string('A', 64);

		_configuration
			.Setup(c => c["TokenKey"])
			.Returns(tokenKey);

		var service =
			new TokenService(
				_configuration.Object,
				_userManager.Object);

		var user = new AppUser
		{
			UserName = null,
			KnownAs = null
		};

		// Act / Assert

		var ex =
			await Assert.ThrowsAsync<Exception>(
				() => service.CreateToken(user));

		Assert.Equal(
			"No username for user",
			ex.Message);
	}
}