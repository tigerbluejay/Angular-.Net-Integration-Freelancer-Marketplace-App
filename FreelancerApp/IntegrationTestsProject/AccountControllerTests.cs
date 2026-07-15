using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using API.IntegrationTests.Infrastructure;
using Xunit;

namespace API.IntegrationTests;

public class AccountControllerTests
	: IClassFixture<CustomWebApplicationFactory>
{
	private readonly HttpClient _client;

	public AccountControllerTests(
		CustomWebApplicationFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task Register_NewUser_Returns_OK()
	{
		// Arrange

		var dto = new RegisterDTO
		{
			Username = "integrationuser",
			Password = "Pa$$w0rd",
			KnownAs = "Jose",
			Role = "Freelancer",

			Gender = "Male",
			Country = "Argentina",
			City = "Buenos Aires",

			DateOfBirth = "March 12 1981"
		};

		// Act

		var response =
			await _client.PostAsJsonAsync(
				"/api/account/register",
				dto);

		// Assert

		var body = await response.Content.ReadAsStringAsync();

		Assert.True(
			response.IsSuccessStatusCode,
			body);

		var user =
			await response.Content
				.ReadFromJsonAsync<UserDTO>();

		Assert.NotNull(user);

		Assert.Equal(
			"integrationuser",
			user!.Username);

		Assert.False(
			string.IsNullOrWhiteSpace(user.Token));
	}
}