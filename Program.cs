using ASPxAngular.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<UserDbContext>(db => db.UseSqlite("Datasource=app.db"));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddAuthentication(o =>
                {
                    o.DefaultScheme = IdentityConstants.BearerScheme;
                    o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                })
                .AddBearerToken(IdentityConstants.BearerScheme)
                .AddCookie(o =>
                {
                    o.Cookie.HttpOnly = true;
                    o.LoginPath = "/login";
                });

builder.Services.AddAuthorizationBuilder();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(IdentityConstants.BearerScheme, o =>
    {
        o.RequireAuthenticatedUser();
    });
    options.AddPolicy(IdentityConstants.ApplicationScheme, o =>
    {
        o.RequireAuthenticatedUser();
    });
    // options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddApiEndpoints();

var app = builder.Build();

app.MapIdentityApi<User>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseCors();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
