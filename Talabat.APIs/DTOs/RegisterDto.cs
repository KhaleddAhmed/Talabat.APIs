﻿using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
	public class RegisterDto
	{

		[Required]
		public string DisplayName { get; set; }

		[Required]
		[EmailAddress] 
		public string Email { get; set; }

		[Required]
        public string Phone { get; set; }

        [Required]
		[RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
			ErrorMessage ="Password must have 1 uppercase, 1 Lowerase, 1 number, 1 non Alphanumeric and at least 6 characters")]
        public string Password { get; set; }
    }
}
