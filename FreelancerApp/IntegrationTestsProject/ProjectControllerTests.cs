using API.Data;
using API.DTOs;
using API.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace API.IntegrationTests;

public class ProjectControllerTests
	: IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _client;

	public ProjectControllerTests(
		CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateProject_AuthenticatedUser_ProjectIsSaved()
	{
		// Arrange

		var loginDto = new LoginDTO
		{
			Username = "freelancer001",
			Password = "Pa$$w0rd"
		};

		var loginResponse =
			await _client.PostAsJsonAsync(
				"/api/account/login",
				loginDto);

		Assert.True(
			loginResponse.IsSuccessStatusCode);

		var loginResult =
			await loginResponse.Content
				.ReadFromJsonAsync<UserDTO>();

		Assert.NotNull(loginResult);
		Assert.False(
			string.IsNullOrEmpty(loginResult!.Token));


		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue(
				"Bearer",
				loginResult.Token);


		var projectDto = new ProjectCreateDTO
		{
			Title = "Integration Test Project",
			Description = "Created from integration test",
			PhotoUrl = "test.jpg",
			Skills = new List<string>
			{
				"C#",
				"ASP.NET Core"
			}
		};


		// Act

		var response =
			await _client.PostAsJsonAsync(
				"/api/project",
				projectDto);


		// Assert

		Assert.Equal(
			HttpStatusCode.Created,
			response.StatusCode);


		var createdProject =
			await response.Content
				.ReadFromJsonAsync<ProjectDTO>();

		Assert.NotNull(createdProject);

		Assert.Equal(
			"Integration Test Project",
			createdProject!.Title);


		// Verify database persistence

		using var scope =
			_factory.Services.CreateScope();

		var context =
			scope.ServiceProvider
				.GetRequiredService<DataContext>();

		var projectExists =
			context.Projects
				.Any(p =>
					p.Id == createdProject.Id);

		Assert.True(projectExists);

		var project = await context.Projects
		.FirstOrDefaultAsync(
			p => p.Id == createdProject.Id);

		Assert.NotNull(project);

		Assert.Equal(
			createdProject.Title,
			project.Title);

		Assert.Equal(
			"freelancer001",
			context.Users
				.Where(u => u.Id == project.ClientUserId)
				.Select(u => u.UserName)
				.Single());
	}
}