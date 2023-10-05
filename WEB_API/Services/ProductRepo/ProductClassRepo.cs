using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WEB_API.DTOs;
using WEB_API.Entities;
using WEB_API.Models.Common;
using WEB_API.Models.Product;

namespace WEB_API.Services.ProductRepo
{
	public class ProductClassRepo : IProductRepo
	{
		private readonly WebApiClass3Context _context;
		public static int PAGE_SIZE { get; set; } = 3;
	public ProductClassRepo(WebApiClass3Context context) {
			_context = context;
		}

		public ProductDTO Add(createProductModel product)
		{
			Product new_product = new Product
			{
				Name = product.name,
				Price = product.price,
				Description = product.description,
				Thumbnail = product.thumbnail,
				Qty = product.qty,
				CategoryId = product.category_id
			};

			_context.Products.Add(new_product);
			_context.SaveChanges();

			Product p = _context.Products
				.Where(p => p.Name == product.name)
				.Include(p => p.Category)
				.First();


			return new ProductDTO
			{
				Id = p.Id,
				Name = p.Name,
				Price = p.Price,
				description = p.Description,
				thumbnail = p.Thumbnail,
				qty = p.Qty,
				category_id = p.CategoryId,
				category = new CategoryDTO
				{
					id = p.Category.Id,
					name = p.Category.Name,
				}
			};

		}

		public void delete(int id)
		{
			var product = _context.Products.FirstOrDefault(p => p.Id == id);

			if (product != null)
			{
				_context.Products.Remove(product);
				_context.SaveChanges();
			}
		}


		// lấy all: trả về một mảng
		public List<ProductDTO> GetAll()
		{
			var products = _context.Products 
				.Include(p => p.Category)
				.ToList();

			List<ProductDTO> ListDTO = new List<ProductDTO>();

			foreach (var product in products) {
				ListDTO.Add(new ProductDTO()
				{
					Id = product.Id,
					Name = product.Name,
					Price = product.Price,
					description = product.Description,
					thumbnail = product.Thumbnail,
					qty = product.Qty,
					category_id = product.CategoryId,
					category = new CategoryDTO()
					{
						id = product.Category.Id,
						name = product.Category.Name,
					}
				});
			}

			return ListDTO;
		}

		// lấy id trả về null or 1 object
		public ProductDTO getById(int id)
		{
			Product p = _context.Products
				.Where(p => p.Id == id)
				.Include(p => p.Category)
				.First();

			if (p == null)
			{
				return null;
			}

			return new ProductDTO()
			{
				Id = p.Id,
				Name = p.Name,
				Price = p.Price,
				description= p.Description,
				thumbnail = p.Thumbnail,
				qty = p.Qty,
				category_id = p.CategoryId,
				category = new CategoryDTO()
				{
					id = p.Category.Id,
					name = p.Category.Name,
				}
			};
		}

		public void update(ProductRepoModel product)
		{
			var product_new = _context.Products.FirstOrDefault(p => p.Id == product.id);

			if(product_new != null)
			{
				product_new.Name = product.name;
				product_new.Price = product.price;
				product_new.Description = product.description == ""? product.description : product_new.Description;
				product_new.Thumbnail = product.thumbnail == ""? product.thumbnail : product_new.Thumbnail;
				product_new.Qty = product.qty;
				product_new.CategoryId = product.category_id;
				_context.SaveChanges();
			}
		}

		// search trả về một mảng
		// vì là kiểu description trong server của tôi là kiểu trường text -> nên .Contains(string) ko thể dùng được với trường text
		// có 2 cách giải quyết một là đổi kiểu dữ liệu trong server trường description thành nvarchar(max)
		// 2 là cách dưới là: dụng hàm EF.Functions.Like kết hợp với hàm EF.Property<string> để chuyển đổi giá trị của cột sang kiểu dữ liệu string trước khi tìm kiếm 
		public List<ProductDTO> Search(string search)
		{
			var allProducts = _context.Products
				.Where(product => product.Name.Contains(search) || EF.Functions.Like(EF.Property<string>(product, "Description"), $"%{search}%") )
				.Include(p => p.Category)
				.ToList();

			List<ProductDTO> Pdto = new List<ProductDTO>();

			foreach (var product in allProducts)
			{
				Pdto.Add(new ProductDTO() { 
					Id = product.Id, 
					Name = product.Name, 
					Price = product.Price,
					description = product.Description,
					thumbnail = product.Thumbnail,
					qty = product.Qty,
					category_id = product.Category.Id,
					category = new CategoryDTO()
					{
						id= product.Category.Id,
						name = product.Category.Name,
					}
				
				});
			}

			return Pdto.Count== 0?null:Pdto;

		}


		public List<Product> Relateds(int id)
		{
			Product p = _context.Products.Find(id);

			if (p == null)
			{
				return null;
			}

			List<Product> products = _context.Products
			.Where(p => p.CategoryId == p.CategoryId)
				.Where(p => p.Id != id)
				.Include(p => p.Category)
				.Take(4)
				.OrderByDescending(p => p.Id) // take products newest
				.ToList();

					/*.Where(pd => pd.CategoryId == p.CategoryId)
				.Where(pd => pd.Id != id)
				.Include(p => p.Category) // cai nao co khoa ngoai phai include khoa ngoai 
				.Take(4)
				.OrderByDescending(pd => pd.Id) // lay giam dan tu 7 6 5 4 3 ...
				.ToList();*/
				

			return products;
		}



		// một hàm ko được vượt quá 3 tham số nhưng vì đây là một function demo nên ta test thử:
		public List<ProductDTO> SFSP(string? search, double? from, double? to, string? sortBy, int page) // page parameter has default value is: 1 we assign result in controller
		{
			// THỰC HIỆN TẠO MỘT TRUY VẤN CÓ THỂ HOLD ĐỢI CÁC THUỘC TÍNH KHÁC  (bằng cách sd hàm .AsQueryable() ) LẤY VỀ ALL DATA
			// SAU ĐÓ SẼ SEARCH FILTER SORTING PAGINATION:
			var allProducts = _context.Products.AsQueryable();

			// Câu hỏi đặt ra: tại sao ko dùng .List() mà phải dùng .AsQueryable() 
			// tại vì: nếu dùng .List() dữ liệu sẽ được query ngay lập tức và ko đợi các thuộc tính bên dưới ...
			// còn .AsQueryable() sẽ query dữ liệu rồi nó để đấy đợi các thuộc tính ở dưới làm xong rồi cùng nhau đưa lên SV or trả về CLIENT


			// B2 kiểm tra nếu có search thì search sản phẩm
			#region SEARCHING
			if (!string.IsNullOrEmpty(search))
			{
				allProducts = allProducts.Where(p => p.Name.Contains(search));
			}
			#endregion

			//B3 kiểm tra nếu có lọc thì lọc sản phẩm
			#region FILTERING
			if (from.HasValue)
			{
				  allProducts = allProducts.Where(p => p.Price >= Convert.ToDecimal(from));
			}

			if (to.HasValue)
			{
				allProducts = allProducts.Where(p => p.Price <= Convert.ToDecimal(to));
			}
			#endregion

			//B4 kiểm tra nếu có sorting thì sorting theo Name, Price... còn ko mặc định sẽ sắp xếp theo bảng chữ cái tăng dần
			#region SORTING
			// sort theo tên product tăng dần nghĩa là: khi chúng ta bấm nút một cái các data sẽ được sort lại gọn gàng:
			allProducts = allProducts.OrderBy(p => p.Name);
			if (!string.IsNullOrEmpty(sortBy))
			{
				// chúng ta sẽ chuyền "key" vào tham số "sortBy" để lấy được kiểu sort mình muốn:
				// ví dụ: ở client ta gán các giá trị default là" "NAME_DESC", "PRICE_ASC"...
				// Nếu chúng ta muốn sắp xếp theo kiểu nào chúng ta chỉ cần truyền "PRICE_ASC" vào tham số "sortBy" thì case đó sẽ chạy!
				switch (sortBy)
				{
					case "NAME_DESC":	allProducts = allProducts.OrderByDescending(p => p.Name); break;
					case "PRICE_ASC":	allProducts = allProducts.OrderBy(p => p.Price); break;
					case "PRICE_DESC":  allProducts = allProducts.OrderByDescending(p => p.Price); break;
				}	

			}
			#endregion

			//B5 tạo Paginate:
			#region PAGING
			// đây là paging theo CÁCH 1:
			allProducts = allProducts.Skip((page - 1)*PAGE_SIZE).Take(PAGE_SIZE);

			// PAGE_SIZE là độ dài của phần tử trong 1 trang
			#endregion


			allProducts.Include(p => p.Category).ToList();
			// xử lý logic 

			List<ProductDTO> Pdto = new List<ProductDTO>();
			foreach (var product in allProducts)
			{
				Pdto.Add(new ProductDTO()
				{
					Id = product.Id,
					Name = product.Name,
					Price = product.Price,
					description = product.Description,
					thumbnail = product.Thumbnail,
					qty = product.Qty,
					category_id = product.Category.Id,
					category = new CategoryDTO()
					{
						id = product.Category.Id,
						name = product.Category.Name,
					}

				});
			}
			return Pdto.Count == 0 ? null : Pdto;
		}



		public List<Product> Paging(int page, int pageSize)
		{
			var allproduct = _context.Products.AsQueryable().Include(p=>p.Category);

			
				var result = PaginationList<Product>.Create(allproduct, page, pageSize > 3?pageSize:PAGE_SIZE);
			
			return result.Select( p => new Product {
					Name = p.Name, 
					Price = p.Price,
					Description = p.Description,
					Thumbnail = p.Thumbnail,
					Qty = p.Qty,
					CategoryId = p.CategoryId,
					Category = new Category()
					{
						Id = p.Category.Id,
						Name = p.Category.Name,
					}
					
			} ).ToList();
			

			
		}


	}
}
