namespace TestWebsite.Components.Pages
{
	public partial class Home
	{
		public class UserDto
		{
			public string Name { get; set; } = "";
			public string Email { get; set; } = "";
			public DateTime Created { get; set; }
			public bool Active { get; set; }
		}

		private List<UserDto> users =
		[
			new() { Name = "Alice", Email = "alice@test.com", Created = DateTime.UtcNow.AddDays(-5), Active = true },
			new() { Name = "Bob", Email = "bob@test.com", Created = DateTime.UtcNow.AddDays(-10), Active = false }
		];
	}
}