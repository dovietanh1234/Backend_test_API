namespace WEB_API.DTOs
{
	public class ProductDTO
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Price { get; set; }

		public string? description { get; set; }

		public string? thumbnail { get; set; }

		public int qty { get; set; }

		public int category_id { get; set; }

		public virtual CategoryDTO category { get; set; }

	}
}

/* virtual dùng cho kiểu đối tượng */