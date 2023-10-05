using System;
using System.ComponentModel.DataAnnotations;

namespace WEB_API.Models.Category
{
	public class CreateCategory
	{
		[Required(ErrorMessage = "please enter category name")]
		[MinLength(3, ErrorMessage = "enter min 3 character ")]
		[MaxLength(255, ErrorMessage ="enter max 255 character")]
		public string name { get; set; }
	}
}
