using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{

	public class AccountController : BaseApiController
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IAuthService _authService;
		private readonly IMapper _mapper;

		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IAuthService authService,
			IMapper mapper
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_authService = authService;
			_mapper = mapper;
		}


		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user is null)
				return Unauthorized(new ApiResponse(401, "Invalid login"));


			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			if (!result.Succeeded)
				return Unauthorized(new ApiResponse(401, "Invalid Login"));

			return Ok(new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _authService.CreateTokenAsync(user, _userManager)
			}); ;

		}



		[HttpPost("register")]

		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			var user = new ApplicationUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				UserName = model.Email.Split("@")[0],
				PhoneNumber = model.Phone

			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded)
				return BadRequest(new ApiValidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) });

			return Ok(new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _authService.CreateTokenAsync(user, _userManager)

			});
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
			var user = await _userManager.FindByEmailAsync(email);

			return Ok(new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _authService.CreateTokenAsync(user, _userManager)
			});
		}


		[Authorize]
		[HttpGet("address")]
		public async Task<ActionResult<AddressDto>> FindUserWithAddress()
		{
			var user=await _userManager.FindUserWithAddressByEmailAsync(User);

			var mappedAddress=_mapper.Map<AddressDto>(user.Address);
			return Ok(mappedAddress);

		}


		[Authorize]
		[HttpPut("address")]
		public async Task<ActionResult<Address>> UpdateUserAddress(AddressDto dto)
		{
			var updatedAddress = _mapper.Map<Address>(dto);
			var user = await _userManager.FindUserWithAddressByEmailAsync(User);
			updatedAddress.Id = user.Address.Id;
			user.Address = updatedAddress;

			var result = await _userManager.UpdateAsync(user);
			if(! result.Succeeded)
			{
				return BadRequest(new ApiValidationErrorResponse() { Errors=result.Errors.Select(E=>E.Description)});
			}

			return Ok(dto);
		}

	}
}
