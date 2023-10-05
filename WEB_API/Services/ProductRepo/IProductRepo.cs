using WEB_API.DTOs;
using WEB_API.Entities;
using WEB_API.Models.Product;

namespace WEB_API.Services.ProductRepo
{
	public interface IProductRepo
	{
		List<ProductDTO> GetAll();

		ProductDTO getById(int id);

		ProductDTO Add(createProductModel product);

		void update(ProductRepoModel product);

		void delete(int id);

		List<ProductDTO> Search(string search);

		List<Product> Relateds(int id);

		// Dấu ? được thêm vào kiểu giá trị của tham số để biểu thị rằng tham số đó có thể nhận giá trị null
		List<ProductDTO> SFSP(string? search, double? from, double? to, string? sortBy, int page);

		List<Product> Paging(int page, int pageSize);
	}
}
