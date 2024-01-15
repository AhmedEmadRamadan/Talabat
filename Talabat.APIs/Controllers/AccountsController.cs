using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Service;

namespace Talabat.APIs.Controllers
{
    public class AccountsController : APIBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result)
                return BadRequest(new ApiResponse(400, "Email Is Already Exist"));
            var User = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
            };
            var Result = await _userManager.CreateAsync(User, model.Password);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
            var ReturnedUser = new UserDto()
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await _tokenService.CreateTokenAsync(User, _userManager)
            };
            return Ok(ReturnedUser);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var User = await _userManager.FindByEmailAsync(model.Email);
            if (User == null) return Unauthorized(new ApiResponse(401));
            var Result = await _signInManager.CheckPasswordSignInAsync(User, model.Password, false);
            if (!Result.Succeeded) return Unauthorized(new ApiResponse(400));
            var ReturnedUser = new UserDto()
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await _tokenService.CreateTokenAsync(User, _userManager)
            };
            return Ok(ReturnedUser);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(Email);
            var ReturnedUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };
            return Ok(ReturnedUser);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("CurrentUserAddress")]
        public async Task<ActionResult<AddressDTO>> GetCurrentUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            var MappedUser = _mapper.Map<Address, AddressDTO>(user.Address);
            return Ok(MappedUser);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("Address")]
        public async Task<ActionResult<AddressDTO>> UpdateAddress(AddressDTO updatedAddress)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            if(user == null) return Unauthorized(new ApiResponse(401));
            var address = _mapper.Map<AddressDTO, Address>(updatedAddress);
            address.Id = user.Address.Id;
            user.Address = address;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(updatedAddress);
        }

        [HttpGet("EmailExists")]
        public async Task<bool> CheckEmailExists(string email)
            => await _userManager.FindByEmailAsync(email) is not null;
    }
}
