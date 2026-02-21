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

	if (!db.Users.Any())
	{
		db.Users.AddRange(
			new User { Name = "Alice", Email = "alice@test.com", Created = DateTime.UtcNow.AddDays(-5), Active = true },
			new User { Name = "Bob", Email = "bob@test.com", Created = DateTime.UtcNow.AddDays(-10), Active = false },
			new User { Name = "Charlie", Email = "charlie@test.com", Created = DateTime.UtcNow.AddDays(-1), Active = true }
		);

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
