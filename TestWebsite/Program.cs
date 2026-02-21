using Microsoft.EntityFrameworkCore;
using TestWebsite.Components;
using TestWebsite.Database;


SQLitePCL.Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddDbContext<TestDbContext>(options => options.UseSqlite("Data Source=test.db"))
	.AddRazorComponents()
	.AddInteractiveServerComponents();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<TestDbContext>();
	db.Database.EnsureCreated();

	//db.Users.ExecuteDelete();

	if (!db.Users.Any())
	{
		var rng = new Random();

		var users = Enumerable.Range(1, 100000).Select(i => new User
		{
			Name = $"User {i}",
			Email = $"user{i}@example.com",
			Created = DateTime.UtcNow.AddDays(-rng.Next(0, 365)),
			Active = rng.NextDouble() > 0.3
		});

		db.Users.AddRange(users);

		db.SaveChanges();
	}
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
