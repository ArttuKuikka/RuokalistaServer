using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using RuokalistaServer.Data;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

string connectionString =
	$"Host={Environment.GetEnvironmentVariable("DB_Server")};" +
	$"Port={Environment.GetEnvironmentVariable("DB_Port") ?? "5432"};" +
	$"Database={Environment.GetEnvironmentVariable("DB_Database")};" +
	$"Username={Environment.GetEnvironmentVariable("DB_User")};" +
	$"Password={Environment.GetEnvironmentVariable("DB_Password")};" +
	"SSL Mode=Disable;" +
	"Trust Server Certificate=true;";


builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(connectionString));
#if DEBUG
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
#endif

builder.Services.AddControllers();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
	var onlySecondJwtSchemePolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
	options.AddPolicy("OnlyJwtScheme", onlySecondJwtSchemePolicyBuilder
		.RequireAuthenticatedUser()
		.Build());
	var onlyCookieSchemePolicyBuilder = new AuthorizationPolicyBuilder("Identity.Application");
	options.AddPolicy("OnlyCookieScheme", onlyCookieSchemePolicyBuilder
		.RequireAuthenticatedUser()
		.Build());
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = "MultiAuthSchemes";
	options.DefaultChallengeScheme = "MultiAuthSchemes";
	options.DefaultScheme = "MultiAuthSchemes";
}).AddPolicyScheme("MultiAuthSchemes", "Bearer", options =>
{
	options.ForwardDefaultSelector = context =>
	{
		string authorization = context.Request.Headers[HeaderNames.Authorization];
		if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
		{
			var token = authorization.Substring("Bearer ".Length).Trim();
			var jwtHandler = new JwtSecurityTokenHandler();
			return (jwtHandler.CanReadToken(token) && jwtHandler.ReadJwtToken(token).Issuer.Equals(Environment.GetEnvironmentVariable("JWT_Issuer")))
				? JwtBearerDefaults.AuthenticationScheme : "Bearer";
		}
		return "Identity.Application";
	};
}).AddJwtBearer(options =>
{
	options.SaveToken = true;
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidAudience = Environment.GetEnvironmentVariable("JWT_Audience"),
		ValidIssuer = Environment.GetEnvironmentVariable("JWT_Issuer"),
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_Secret") ?? throw new Exception("No JWT_Secret set!")))
	};
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(e => { e.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Ruokalista API", Version = "v1" }); });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Ensure database is created and add default user if it was just created
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<ApplicationDbContext>();
	context.Database.Migrate();

	if (!context.Users.Any())
	{
		var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
		var userName = Environment.GetEnvironmentVariable("RootUser") ?? throw new Exception("No RootUser environment variable provided");
		var pass = Environment.GetEnvironmentVariable("DefaultRootPassword") ?? throw new Exception("No DefaultRootPassword environment variable provided");

		var user = new IdentityUser { UserName = userName, Email = userName };
		var result = userManager.CreateAsync(user, pass).Result;

		if (!result.Succeeded)
		{
			throw new Exception("Failed to create default user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
		}
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/");

app.MapRazorPages();

app.Run();
