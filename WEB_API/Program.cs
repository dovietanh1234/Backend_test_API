using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WEB_API.Services;
using WEB_API.Services.ProductRepo;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Lấy chuỗi secretKey từ file appsettings.json -> bind vào file Models/AppSetting.cs:
//Cách 1: var secretKey = builder.Configuration["AppSetting:SecretKey"]
// var secretKeyBytes = Encoding.UTF8.GetBytes(SecretKey);  // vì thuật toán mã hoá của mình chỉ sử dụng trên Bytes nên ta phải ép kiểu
//Cách 2:
string secretKey = builder.Configuration.GetConnectionString("SecretKey");
// sau đó mã hoá thành string -> mã Bytes (0101110101 số nhị phân) để phù hợp với thuật toán mã hoá:
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

//B3 Configure Authorization service(cấu hình dịch vụ authorization) :
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
	option.TokenValidationParameters = new TokenValidationParameters
	{
		// tự Cấp token: nếu sd dịch vụ cấp token (tạo 1 lần dùng ở nhiều nơi like gmail ...) thì cần cấu hình khác, phải config tới phần bạn chọn
		ValidateIssuer = false,
		ValidateAudience = false,

		// ký vào token bằng chuỗi secretKey(Bytes):
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // Symmetric algorithm

        ClockSkew = TimeSpan.Zero,
	};
});


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

// sử dụng gọi lại cấu hình của core ra đây:
app.UseCors();

// sử dụng -> gọi lại cấu hính của authentication ở trên ra đây: | authentication luôn luôn đứng trước authorization(xác thực) trước phân quyền
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
