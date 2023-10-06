using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_API.Models.Product;
using WEB_API.Services.ProductRepo;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using WEB_API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace WEB_API.Controllers.Product_controller
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductRepoController : ControllerBase
	{
		private readonly IProductRepo _repo_product;

		public ProductRepoController(IProductRepo repo_product)
		{
			_repo_product = repo_product;
		}

		[HttpGet]
		public IActionResult GetAllP()
		{
			try
			{

				return Ok(_repo_product.GetAll());

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/* 	nếu trong một router viết thế này sẽ bị lỗi link API -> ta sẽ chỉ đặt get không thôi! 
		 * 	[HttpGet("{id}")]
			[Route("detail")]
		 */

		[HttpGet]
		[Route("get-by-id")]
		public IActionResult GetDetailP(int id) {
			try
			{

				var data = _repo_product.getById(id);
				if(data != null)
				{
					return Ok(data);
				}

				return NotFound();

			}catch (Exception ex) { 
				return BadRequest(ex.Message);	
			}
		
		}



		[HttpPut("{id}")]
		public IActionResult updateP(int id, ProductRepoModel product)
		{
			if (id != product.id)
			{
				return Ok("data is not match together");
			}

			try
			{
				_repo_product.update(product);
				return NoContent();
	
			}catch(Exception ex) { 
				return BadRequest(ex.Message);
			
			}
		}




		[HttpPost]
		[Authorize]
		public IActionResult AddP(createProductModel product)
		{
			try
			{
				return Ok(_repo_product.Add(product));
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}



		// khi bạn dùng thằng Get để nhận vào một tham số trong hàm -> thì bạn phải đưa key là tên tham số vào trong phân annotation method GET... 
		//[HttpGet("{search}")]
		[HttpGet]
		[Route("get-by-search")]
		public IActionResult SearchP(string search)
		{
			try
			{
				if(search != null)
				{
					// một kiểu return về giá trị:	return _repo_product.Search(search)==null ? NotFound(): Ok(_repo_product.Search(search));

					var result = _repo_product.Search(search);
					if(result != null)
					{
						return Ok(result);
					}
					return NotFound();
					
				}

				return NotFound();
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}

		[HttpGet]
		[Route("related")]
		public IActionResult Related(int id)
		{
			try
			{

				var result = _repo_product.Relateds(id);
				if(result != null)
				{
					return Ok(result);
				}
				return NotFound();

			}catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Route("SFSP")]
		public IActionResult SFSP(string? search, double? from, double? to, string? sortBy, int page=1)
		{
			try
			{

				var result = _repo_product.SFSP(search, from, to, sortBy, page);
				if (result != null)
				{
					return Ok(result);
				}
				return NotFound();


			}
			catch(Exception ex) {
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Route("paginate2")]
		public IActionResult Paginate(int page = 1, int pageSize = 1)
		{
			try
			{
				return Ok( _repo_product.Paging(page, pageSize));
			}catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


	}
}
