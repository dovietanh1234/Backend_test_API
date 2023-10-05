using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_API.Models.Category;
using WEB_API.Services;

namespace WEB_API.Controllers
{
	[Route("api/repository/[controller]")]
	[ApiController]
	public class CateControllerRepository : ControllerBase
	{
		private readonly ICategoryRepository _interfaceCate;
		// sau khi khai báo controller interface xong! thì ta cần đăng ký trong repository trong DI Container -> để sử dụng interface mà ko cần khai báo
		// vào file startup.cs (version .NET 6 tích hợp file startup.cs -> vào file program.cs) -> đăng ký:
		//  builder.Services.AddScoped<ICategoryRepository, CategoryClassRepository>();
		public CateControllerRepository(ICategoryRepository interfaceCate)
		{
			_interfaceCate = interfaceCate;
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			try
			{
				// gọi đến class chứa các actions mà kế thừa interface chúng ta sẽ không phải new khởi tạo đối tượng để gọi đến thuộc tính của nó mà vì 
				//ta đã khai báo trong DI Container rồi nên ta sẽ từ interface có thể gọi đến luôn class chứa actions interface 
				return Ok(_interfaceCate.GetAll() );

			}catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				// gọi đến class chứa các actions mà kế thừa interface chúng ta sẽ không phải new khởi tạo đối tượng để gọi đến thuộc tính của nó mà vì 
				//ta đã khai báo trong DI Container rồi nên ta sẽ từ interface có thể gọi đến luôn class chứa actions interface 
				var data = _interfaceCate.GetById(id);
				if(data != null)
				{
					return Ok(data);
				}
				return NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		[HttpPut("{id}")]
		public IActionResult Update(int id, EditCategory categpry)
		{
			if(id != categpry.id)
			{
				return BadRequest("id is not match");
			}
			try
			{
				// gọi đến class chứa các actions mà kế thừa interface chúng ta sẽ không phải new khởi tạo đối tượng để gọi đến thuộc tính của nó mà vì 
				//ta đã khai báo trong DI Container rồi nên ta sẽ từ interface có thể gọi đến luôn class chứa actions interface 
				 _interfaceCate.Update(categpry);
				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_interfaceCate.Delete(id);
				return Ok();

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		public IActionResult Add(CreateCategory category)
		{
			try
			{

				return Ok(_interfaceCate.Add(category));
			}catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		


	}
}
