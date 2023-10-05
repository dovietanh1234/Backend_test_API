using Microsoft.EntityFrameworkCore;
using WEB_API.Services;
using WEB_API.Services.ProductRepo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// CONNECT DB:
string connectionString = builder.Configuration.GetConnectionString("API");
builder.Services.AddDbContext<WEB_API.Entities.WebApiClass3Context>(
			options => options.UseSqlServer(connectionString)
);

//ADD CORS:
builder.Services.AddCors( options =>
{
	options.AddDefaultPolicy(policy =>
		{
			policy.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader();
		});
});

// add services container: dimiss limit amount json
builder.Services.AddControllers().AddNewtonsoftJson( options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore );

// khai báo interface và hàm th?c hi?n các actions c?a interface trong service:
builder.Services.AddScoped<ICategoryRepository, CategoryClassRepository>();
builder.Services.AddScoped<IProductRepo, ProductClassRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
