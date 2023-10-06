namespace WEB_API.Models.Common
{
    public class AppSetting
    {
        public string SecretKey { get; set; }
        public AppSetting()
        {

        }

    }
}

/*
 * Nếu làm theo cách cấu hình "secretKey" cách 1 thì nó sẽ tự động bind vào đây và lấy nhưng ta làm theo cách 2 nên ta sử dụng cách khác:
 Nơi đây sẽ chứa mã secretKey tự đọng được inject vào controller: 

đây là cách file config lấy chuỗi string db:
string connectionString = builder.Configuration.GetConnectionString("API");


 */