using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_API.DTOs;
using WEB_API.Entities;
using WEB_API.Models.Category;

namespace WEB_API.Controllers
{
	
	[ApiController]
	[Route("api/category")]
	public class CategoryController : ControllerBase
	{
		private readonly WebApiClass3Context _context;
		public CategoryController( WebApiClass3Context context) {
		
			_context = context;

		}

		[HttpGet]
		public IActionResult Index()
		{
			List<Category> categories = _context.Categories.ToList();

			List<CategoryDTO> data = new List<CategoryDTO>();

			foreach (Category c in categories)
			{
				data.Add(new CategoryDTO {
					id = c.Id,
					name = c.Name,
				});
			}
			return Ok(data);


		}


		[HttpGet]
		[Route("get-by-id")] // localhost111/get-by-id/{id}
		public IActionResult Get(int id)
		{
			try
			{
				Category c = _context.Categories.Find(id);

				if ( c != null )
				{
					// forces the type to DTO return data for client 
					return Ok( new CategoryDTO { 
						id = c.Id,
						name = c.Name
					} );
				}
			}catch (Exception ex)
			{
				return BadRequest( ex.Message );
			}
			return NotFound("not found");
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN")]
		public IActionResult Create(CreateCategory model)
		{
			if ( ModelState.IsValid )
			{
				try
				{
					// tạo một đối tượng từ Entities.Category và đưa dữ liệu vào cho đói tượng sau đó đưa dữ liệu vào DB
					Category data = new Category { Name = model.name };
					_context.Categories.Add(data);
					_context.SaveChanges();

					// gọi method "Created" đưa vào 2 tham số 1: string URL | 2: đối tượng or object chứa dữ liệu để nó trả về
					return Created($"get-by-id?id={data.Id}", new CategoryDTO { id = data.Id, name = data.Name });

				}
				catch(Exception ex)
				{
					return BadRequest(ex.Message );
				}
			}

			// trả về mã lỗi cho người dùng:
			var msgs = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);
			return BadRequest(string.Join(" | ", msgs));

		}


		[HttpPut] // ham nay bi sai
		public IActionResult Update(EditCategory model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					// muốn đưa dữ liệu vào _context() thì ta cần dùng entities.Category
					// tạo đối tượng category -> đưa vào dataContext ... 
					var categoryN = _context.Categories.SingleOrDefault(cate => cate.Id == model.id);

					categoryN.Name = model.name;
					_context.SaveChanges();
					return NoContent();
				}
				catch (Exception ex)
				{
					return BadRequest(ex.Message);
				}

			}
			return BadRequest();
		}


		[HttpDelete]
		public IActionResult Delete(int id)
		{
			try
			{

				Category category = _context.Categories.Find(id);

				if (category == null)
				{
					return NotFound();	
				}
				_context.Categories.Remove(category);
				_context.SaveChanges();
				return NoContent();

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}


/*
 hàm Created():hàm Created nhận vào hai tham số: một chuỗi biểu diễn đường dẫn để lấy tài nguyên
mới với id là data.Id, và một đối tượng CategoryDTO chứa hai thuộc tính id và name của tài nguyên mới. 
hàm Created(): thường được sử dụng trong các API để thông báo cho người dùng rằng một tài nguyên mới 
đã được tạo thành công trên máy chủ và cung cấp một đường dẫn để truy cập tài nguyên đó
 */