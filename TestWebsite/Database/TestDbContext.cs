using Microsoft.EntityFrameworkCore;


namespace TestWebsite.Database
{

	public class TestDbContext : DbContext
	{
		public TestDbContext(DbContextOptions<TestDbContext> options)
			: base(options) { }

		public DbSet<User> Users => Set<User>();
	}

	public class User
	{
		public uint Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public DateTime Created { get; set; }
		public bool Active { get; set; }
	}
}
