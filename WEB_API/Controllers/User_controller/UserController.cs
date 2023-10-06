using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WEB_API.Entities;
using WEB_API.Models.Common;
using WEB_API.Models.Login;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;

namespace WEB_API.Controllers.User_controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //assignment to the field can only occur as part of the declaration or in a constructor in the same class
        private readonly WebApiClass3Context _context;
        //lấy chuỗi secrekey trong appsetting.json
        private readonly IConfiguration _configuration;

    
        public UserController(WebApiClass3Context context, IConfiguration configuration) { 
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Validate(LoginModel model)
        {
            var user = _context.User2s.SingleOrDefault( p => p.Name == model.Name && p.Password == model.Password );

            if (user == null)
            {
                return Ok(new ApiResponse // vi class nay ko co controller nen ta phai gan gia tri thu cong!
                {
                    status = 401,
                    message = "password or username is wrong"
                });
            }

           return Ok( new ApiResponse
            {
                status= 200,
                message = "login successfully",
                data = GenerateToken(user) // create token
            } );
        }


        // Tạo một method create token: nếu đúng thì ta phải viết ở service nhưng viết ở đây cho nhanh:
        private string GenerateToken(User2 user)
        {
            // tạo new 1 đối tượng  JwtSecurityTokenHandler -> dùng đối tượng này chuyền vào các options để tạo token:
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // lấy chuỗi secret key mã hoá:
            var secretKeyByte = Encoding.UTF8.GetBytes(_configuration.GetSection("ConnectionStrings:SecretKey").Value);

            // phát sinh ra token: tạo mới một secutity description
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("Id", user.Id.ToString())

                    // roles sẽ để ở chỗ này
                }),
                // expire time:
                Expires = DateTime.UtcNow.AddMinutes(1),
                // add signature type is Byte in here( thêm chữ ký ) + "HmacSha512Signature" algorithm handles on byte type variable:
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyByte), SecurityAlgorithms.HmacSha512Signature)
            };

            // tạo mã token với các options của mình
            var token = jwtTokenHandler.CreateToken(tokenDescription);

            // trả về cho ta một cái chuỗi:
            return jwtTokenHandler.WriteToken(token);
        }



    }
}
