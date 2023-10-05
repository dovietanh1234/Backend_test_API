using System.ComponentModel.DataAnnotations;

namespace WEB_API.Models.Product
{
	public class ProductRepoModel
	{
		public int id { get; set; }

		[Required(ErrorMessage = "please enter product name")]
		[MinLength(3, ErrorMessage = "enter min 3 character ")]
		[MaxLength(255, ErrorMessage = "enter max 255 character")]
		public string name { get; set; }

		[Required(ErrorMessage = "please enter price product")]
		public int price { get; set; }
		
		public string description { get; set; }

		public string thumbnail { get; set; }

		[Required(ErrorMessage = "please enter quantity")]
		public int qty { get; set; }

		[Required(ErrorMessage = "please enter type of category")]
		public int category_id { get; set; }	
	}
}
