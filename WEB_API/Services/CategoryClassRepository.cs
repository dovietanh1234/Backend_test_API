using Microsoft.AspNetCore.Http.HttpResults;
using WEB_API.Entities;
using WEB_API.Models.Category;

namespace WEB_API.Services
{
	public class CategoryClassRepository : ICategoryRepository
	{
		private readonly WebApiClass3Context _context;
		public CategoryClassRepository(WebApiClass3Context context) {
			_context = context;
		}
		public CategoryRepository Add(CreateCategory category)
		{
			// lưu trong DB thì phải gọi đúng entity của nó là Category
			var category_new = new Category
			{
				Name = category.name
			};
			_context.Categories.Add(category_new);
			_context.SaveChanges();

			return new CategoryRepository
			{
				id = category_new.Id, 
				name = category.name
			};
		}

		public void Delete(int id)
		{
			var category = _context.Categories.FirstOrDefault(cate => cate.Id == id);

			if(category != null)
			{
				_context.Categories.Remove(category);
				_context.SaveChanges();
			}
		}

		public List<CategoryRepository> GetAll()
		{
			// lấy dữ liệu ra rồi ép kiểu về form model mới của mình trả về cho client:
			var categories = _context.Categories.Select(cate => new CategoryRepository { 
				id = cate.Id, 
				name = cate.Name 
			});

			return categories.ToList();
			
		}

		public CategoryRepository GetById(int id)
		{
			var category = _context.Categories.FirstOrDefault(cate => cate.Id == id);

			if (category != null)
			{
				return new CategoryRepository { 
					id = category.Id, 
					name = category.Name 
				};
			}
			return null;
		}

		public void Update(EditCategory categpry)
		{
			var category_new = _context.Categories.FirstOrDefault(cate => cate.Id == categpry.id );

			if (category_new != null)
			{
				category_new.Name = categpry.name;
				_context.SaveChanges();
			}
		}
	}
}
