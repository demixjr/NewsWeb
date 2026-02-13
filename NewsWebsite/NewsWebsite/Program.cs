using AutoMapper;
using BLL.Interfaces;
using BLL;
using BLL.Services;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Configure DbContext
builder.Services.AddDbContext<NewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile), typeof(Program));

// Register Repositories (внутрішня залежність BLL)
builder.Services.AddScoped<IRepository<News>, Repository<News>>();
builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();

// Register Services (UI працює ТІЛЬКИ з ними)
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add Session support for user authentication
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages()
    .AddMvcOptions(opts =>
    {
        opts.Filters.AddService<CategoryNavFilter>();
    });

builder.Services.AddScoped<NewsWebsite.Filters.CategoryNavFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();