using System.ComponentModel.DataAnnotations;

namespace HardDelivery.Models
{
	public class Address
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[StringLength(4), MinLength(4)]
		public int PostCode { get; set; }
		[Required]
		public string? StreetN { get; set; }

	}
}
