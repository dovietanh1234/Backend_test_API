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
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Validate(LoginModel model)
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

            var token = await GenerateToken(user);

           return Ok( new ApiResponse
            {
                status= 200,
                message = "login successfully",
                data = token // create token
            } );
        }


        // Tạo một method create token: nếu đúng thì ta phải viết ở service nhưng viết ở đây cho nhanh:
        private async Task<TokenModel> GenerateToken(User2 user)
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
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    // roles sẽ để ở chỗ này
                    // ở bài next table refreshToken sẽ có thêm một trường là cái id accessToken sẽ thuộc vào cái refresh token nào?

                }),
                // expire time:
                Expires = DateTime.UtcNow.AddSeconds(50),
                // add signature type is Byte in here( thêm chữ ký ) + "HmacSha512Signature" algorithm handles on byte type variable:
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyByte), SecurityAlgorithms.HmacSha512Signature)
            };

            // tạo mã token với các options của mình
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            // trả về cho ta một cái chuỗi:
            var AccessToken = jwtTokenHandler.WriteToken(token);

            //viết tắt
            //var AccessToken = jwtTokenHandler.WriteToken(jwtTokenHandler.CreateToken(tokenDescription));
            var refreshToken = GenerateRefreshToken();

            // save in database:
            var refreshTokenEntity = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IsUsedAt = DateTime.UtcNow, // date create
                ExpiredAt = DateTime.UtcNow.AddHours(1), // expore in 1 hour
            };
           await _context.AddAsync(refreshTokenEntity);
           await _context.SaveChangesAsync();

            return new TokenModel()
            {
                AccessToken = AccessToken,
                RefreshToken = refreshToken
                /* trong quá trình generate ra cái token của mình mình sẽ lưu trữ trên DB nhé! -> lưu trữ trong DB  */
            };

        }


        private string GenerateRefreshToken()
        {
            var random = new byte[32];


            // sử dụng của using statement -> một cấu trúc ngôn ngữ dùng để đảm bảo việc giải phóng tài nguyên của một đối tượng có thể giải phóng (disposable) khi kết thúc một khối lệnh
            // tạo một số random: 
            using (var rng = RandomNumberGenerator.Create())
            {

                rng.GetBytes(random);
                return Convert.ToBase64String(random);
                /*
                 sử dụng using statement, đối tượng rng sẽ được khởi tạo trong phần khai báo, 
                sau đó được sử dụng trong khối lệnh bên trong. Khi kết thúc khối lệnh, 
                đối tượng rng sẽ được gọi phương thức Dispose để giải phóng tài nguyên. 
                Điều này đảm bảo rằng không có rò rỉ bộ nhớ hoặc tài nguyên khi sử dụng các đối tượng có thể giải phóng
                 */
            }

            // TẠO BẢNG REFRESH TOKEN TRÊN DB:
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            // ta có thể viểt cái hàm refresh token riêng cux đc nhưng ta sẽ viết ở đây:

            // tạo new 1 đối tượng  JwtSecurityTokenHandler -> dùng đối tượng này chuyền vào các options để tạo token:
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // lấy chuỗi secret key mã hoá:
            var secretKeyByte = Encoding.UTF8.GetBytes(_configuration.GetSection("ConnectionStrings:SecretKey").Value);





            // lấy TokenValidationParameters:
            var tokenValidationParameter = new TokenValidationParameters
            {
                // tự Cấp token: nếu sd dịch vụ cấp token (tạo 1 lần dùng ở nhiều nơi giong nhu gmail ...) thì cần cấu hình khác, phải config tới phần bạn chọn
                ValidateIssuer = false,
                ValidateAudience = false,

                // ký vào token bằng chuỗi secretKey(Bytes):
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyByte), // Symmetric algorithm

                ClockSkew = TimeSpan.Zero,

                // vì khi token validate nếu token expire là nhảy sang phần catch... nên ta sẽ bỏ cái check ở đây đi để gọi ở dưới nó nhảy vào catch...
                ValidateLifetime = false

            };

            // next: validate xem token này còn hợp lệ hay ko:
            try
            {
                // check 1: validate accesstoken:

                // tham số 1 là: accessToken lấy từ đối tượng TokenModel
                // tham số 2: là params validation parameters: ( file program.cs ở bài 1 ta đã cấu hình Authentication ( bên trong có chứa cấu hình opt.TokenValidationParameters )  )
                // ta sẽ copy TokenValidationParameters từ file program sang đây làm tham số thứ 2:     
                // tham số thứ 3: nó sẽ out ra cho một cái biến "validatetoken" -> nghĩa là biến này chứa token đã đc xác thực rồi
                //  "TokenVerification" sẽ lưu trữ kết quả trả về của phương thức ValidateToken()
                // "validatedtoken" Biến này được sử dụng để lưu trữ token JWT đã được xác thực -> đầu ra của hàm "validateToken" ->Nó sẽ chứa giá trị của token sau khi đã được xác thực

                var TokenVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidationParameter, out var validatedtoken);

                //check 2: khác mã hoá về thuật toán:
                // từ cái validatedToken (token sau khi đc xác thực) của ta -> ta có thể ép kiểu về thành jwtSecurityToken
                if (validatedtoken is JwtSecurityToken jwtSecurityToken) // ep kieu
                {
                    // sau khi ép kiểu ta sẽ đi check lại:
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase); // check thuat toan & ko phan biet hoa thuong

                    if (!result)
                    {
                        return Ok(new ApiResponse
                        {
                            status = 401,
                            message = "token invalid",
                            data = null
                        });
                    }
                }

                // check 3: check accessToken expire or not:
                // dùng để chuyển đổi một chuỗi sang một số nguyên kiểu long
                // parse dữ liệu ra kiểu long -> gọi đến "TokenVerification" -> gọi danh sách Claims ra -> tìm ngày bằng cách sử dụng hàm "FirstOrDefault" -> lấy theo kiểu jwtTokenExpire( JwtRegisteredClaimNames.Exp )
                var utcExpireDate = long.Parse(TokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expirDate = ConvertUnixTimeToDateTime( utcExpireDate ); // dung 1 ham tinh toan thoi gian cua token
                // nếu như khoảng thời gian này lớn hơn thời gian hiện tại ( nghia la khoang thoi gian còn lại của token vẫn còn )
                if (expirDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        status = 401,
                        message = "access token still valid cannot generate new token",
                        data = null
                    });
                }

                //check 4: check refreshtoken is exist in DB: 
                var storedToken = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == model.RefreshToken);
                
                if (storedToken == null)
                {
                    return Ok(new ApiResponse
                    {
                        status = 401,
                        message = "refresh token isn't exist in DB",
                        data = null
                    });
                }


                // check 5: check refreshToken is used or revoke or not?
                if ( storedToken.IsUsed == true || storedToken.IsRevoked == true)
                {
                    return Ok(new ApiResponse
                    {
                        status = 401,
                        message = "refresh token has been used or revoked",
                        data = null
                    });
                }

                // check 6: accessToken id co bang voi JwtId in RefreshToken table
                var jti = TokenVerification.Claims.FirstOrDefault( x => x.Type == JwtRegisteredClaimNames.Jti ).Value;

                if ( storedToken.JwtId != jti)
                {
                    return Ok(new ApiResponse
                    {
                        status = 401,
                        message = "refresh token has been used or revoked",
                        data = null
                    });
                }





                // test thử: check xem role có phải là admin ko?
                // var role_inToken = TokenVerification.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
                // if ( role_inToken != "ADMIN" )
                // {
                //    return Ok(new ApiResponse
                //    {
                //        status = 401,
                //        message = "you are not admin ",
                //        data = null
                //    });
                // }






                // NẾU QUA HẾT BƯỚC CHECK -> TA CÓ THỂ UPDATE TOKEN đã qua sử dụng:
                storedToken.IsUsed = true;
                storedToken.IsRevoked = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();

                // CREATE NEWS:
                var user = await _context.User2s.SingleOrDefaultAsync(user => user.Id == storedToken.UserId);

                var token = await GenerateToken(user);


                return Ok(new ApiResponse
                {
                    status = 200,
                    message = "renew token success",
                    data = token
                });


            }catch(Exception ex) {
                // nếu token expire là sang phần catch

                return Ok(
                    new ApiResponse
                    {
                        status = 401,
                        message = "token invalid",
                        data = ex.Message
                    }
                    );

            }

        }


        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;

        }
    }
}
