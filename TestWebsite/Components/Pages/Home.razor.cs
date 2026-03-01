using Microsoft.AspNetCore.Components;
using TestWebsite.Database;

namespace TestWebsite.Components.Pages
{
	public partial class Home : ComponentBase
	{
		[Inject]
		public TestDbContext DbContext { get; set; }


		public class UserDto
		{
			public string Name { get; set; } = "";
			public string Email { get; set; } = "";
			public DateTime Created { get; set; }
			public bool Active { get; set; }
		}

		private List<UserDto> users;


		protected override void OnInitialized()
		{
			users = GenerateUsers();
		}


		private List<UserDto> GenerateUsers()
		{
			var rng = new Random();

			var users = Enumerable.Range(1, 10).Select(i => new UserDto
			{
				Name = $"User {i}",
				Email = $"user{i}@example.com",
				Created = DateTime.UtcNow.AddDays(-rng.Next(0, 365)),
				Active = rng.NextDouble() > 0.3
			});

			return users.ToList();
		}
	}
}