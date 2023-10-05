using System.ComponentModel.DataAnnotations;

namespace WEB_API.Models.Category
{
	public class EditCategory
	{
		[Required]
		public int id { get; set; }

		[Required(ErrorMessage = "please enter name category")]
		[MinLength(3, ErrorMessage ="enter min 3 character ... ")]
		[MaxLength(255, ErrorMessage = "enter max 255 character ... ")]
		public string name { get; set; }
	}
}
