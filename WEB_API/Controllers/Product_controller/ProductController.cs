using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_API.DTOs;
using WEB_API.Entities;

namespace WEB_API.Controllers.Product_controller
{
	[ApiController]
	[Route("api/product")]
	public class ProductController : ControllerBase
	{
		private readonly WebApiClass3Context _context;

		public ProductController(WebApiClass3Context context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var products = _context.Products
				.Include(p => p.Category) // nap du lieu là (Category) vao cho entities Product
				.ToList();
			// tạo một đối tượng DTO để trả về dữ liệu cho người dùng:
			List<ProductDTO> ls = new List<ProductDTO>();

			// nạp data vào DTO trả về client:
			foreach (var product in products)
			{
				ls.Add(new ProductDTO
				{
					Id = product.Id,
					Name = product.Name,
					Price = product.Price,
					description = product.Description,
					thumbnail = product.Thumbnail,
					qty = product.Qty,
					category_id = product.CategoryId,
					category = new CategoryDTO
					{
						id = product.Category.Id,
						name = product.Category.Name
					}
				});
			}
			return Ok(ls);
		}



		[HttpGet]
		[Route("get-by-id")]
		public IActionResult Get(int id)
		{
			try
			{
				Product p = _context.Products
							.Where(p => p.Id == id)
							.Include(p => p.Category)
							.First();
				if (p == null)
				{
					return NotFound();
				}

				return Ok(new ProductDTO
				{
					Id = p.Id,
					Name = p.Name,
					Price = p.Price,
					description = p.Description,
					thumbnail = p.Thumbnail,
					qty = p.Qty,
					category_id = p.Category.Id,
					category = new CategoryDTO
					{
						id = p.Category.Id,
						name = p.Category.Name
					}

				});

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		// lấy ra các sản phẩm liên quan: vì là trả về một mảng dữ liệu JSON -> nhưng ASP .NET WEB CORE nó limit lượng json trả về 
		// sẽ có lỗi: Ta sẽ tải thư viện NewtonSoft.json và khai báo trong DI container để sử dụng...
		[HttpGet]
		[Route("relateds")]
		public IActionResult Relateds(int id)
		{
			try
			{
				Product p = _context.Products.Find(id);

				if (p == null)
				{
					return NotFound();
				}

				// tạo một list product và trả về cho client:
				List<Product> ls = _context.Products
					.Where(p => p.CategoryId == p.CategoryId)
					.Where(p => p.Id != id)
					.Include(p => p.Category)
					.Take(4)
					.OrderByDescending(p => p.Id) // take products newest
					.ToList(); 
				return Ok(ls);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}


/*
 
List<product> products = _context.Products
	.Include(p=>p.category).toList(); // nạp thêm dữ liệu vào
	.Where(p=>p.name.Equals("samsung")); // lấy dữ liệu có sam sung so sánh chính xác
	.Where(p=>p.name.Contains("samsung") || p.name.Contains("apple")); // lấy dữ liệu có từ sam sung
	.Take(10) //
	.Skip(5) // phải tự nhớ công thức phân trang 
	.OrderBy(p=>p.name)//ascending
	.OrderByDescending(p=>p.name)//descending 

 
.include() là: kỹ thuật Eager Loading, tức là tải trước các dữ liệu liên quan khi truy vấn entity chính.
=> giúp cho Điều này giúp bạn có thể truy cập đến các thuộc tính của entity liên quan mà không cần phải thực hiện một truy vấn khác
 */