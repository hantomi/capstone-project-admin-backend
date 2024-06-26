﻿using DeliverBox_BE.Misc;
using DeliverBox_BE.Models;
using DeliverBox_BE.Objects;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/authorize")]
    [ApiController]
    public class AuthorizeController : Controller
    {
        private readonly IConfiguration _config;
        public AuthorizeController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost(template: "user-login")]
        public ActionResult Login([FromBody] CustomerLoginModel model)
        {
            var user = Authenticate(model);
            if (user != null)
            {
                var result = new { token = GenerateToken(user) };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                return Content(json, "application/json");
            }

            return NotFound("user not found");
        }

        // To generate token
        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.MobilePhone,user.Phone),
                new Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(3600),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //To authenticate user
        private User? Authenticate(CustomerLoginModel model)
        {
            var currentUser = UserConstants.Users.FirstOrDefault(x => x.Phone.Length != 0 && x.Password.Length != 0);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
