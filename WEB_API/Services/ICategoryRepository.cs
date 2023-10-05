using WEB_API.Models.Category;

namespace WEB_API.Services
{
	public interface ICategoryRepository
	{
		
		List<CategoryRepository> GetAll();

		CategoryRepository GetById(int id);

		CategoryRepository Add(CreateCategory category);

		void Update(EditCategory categpry);

		void Delete(int id);
	}
}
