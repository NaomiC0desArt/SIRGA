using Microsoft.AspNetCore.Identity;


namespace SIRGA.Identity.Shared.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public char Gender { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public string? Photo { get; set; }
		public string Province { get; set; }
		public string Sector { get; set; }
		public string Address { get; set; }

		public bool IsActive { get; set; }

		public DateOnly DateOfEntry { get; set; }
		public DateOnly CreatedAt { get; set; }

		public bool MustCompleteProfile { get; set; } = true;
		public DateTimeOffset? LastLogin { get; set; }
	}
}
