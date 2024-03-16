using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SymmetricSecurityKey _secretKey;
    private readonly TimeSpan _accessTokenExpiration;
    private readonly TimeSpan _refreshTokenExpiration;

    public AuthController()
    {
        _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TrainingFresher_ivs@123"));
        _accessTokenExpiration = TimeSpan.FromMinutes(1);
        _refreshTokenExpiration = TimeSpan.FromMinutes(30);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel user)
    {
        if (user is null)
        {
            return BadRequest("Invalid client request");
        }

        if (user.UserName == "long" && user.Password == "1")
        {
            var accessToken = GenerateAccessToken();
            var refreshToken = GenerateRefreshToken();

            var token = new
            {
                accessToken = accessToken,
                refreshToken = new
                {
                    token = refreshToken,
                    expiredAt = DateTime.Now.Add(_refreshTokenExpiration)
                },
                expiredAt = DateTime.Now.Add(_accessTokenExpiration)
            };

            return Ok(new { Token = token });
        }

        return Unauthorized();
    }

    // [HttpPost("refreshtoken")]
    // public IActionResult Refresh([FromBody] RefreshTokenModel refreshTokenModel)
    // {
    //     if (refreshTokenModel is null)
    //     {
    //         return BadRequest("Invalid client request");
    //     }

    //     // Validate the refresh token
    //     if (IsValidRefreshToken(refreshTokenModel.RefreshToken))
    //     {
    //         var accessToken = GenerateAccessToken();

    //         return Ok(new AuthenticatedResponse { AccessToken = accessToken });
    //     }

    //     return Unauthorized();
    // }

    private string GenerateAccessToken()
    {
        var claims = new List<Claim>();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = "http://localhost:5129",
            Audience = "http://localhost:5129",
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_accessTokenExpiration),
            SigningCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(accessToken);
    }

    private string GenerateRefreshToken()
    {
        var refreshToken = Guid.NewGuid().ToString();
        return refreshToken;
    }

    // private bool IsValidRefreshToken(string refreshToken)
    // {
    //     // Check if the refresh token is valid (e.g., compare with stored refresh tokens)

    //     return true; // Placeholder logic, implement actual validation
    // }
}


// OLD CODE
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;

// [Route("api/[controller]")]
// [ApiController]
// public class AuthController : ControllerBase
// {
//     [HttpPost("login")]
//     public IActionResult Login([FromBody] LoginModel user)
//     {
//         if (user is null)
//         {
//             return BadRequest("Invalid client request");
//         }

//         if (user.UserName == "long" && user.Password == "1")
//         {
//             var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
//             var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
//             var tokeOptions = new JwtSecurityToken(
//                 issuer: "http://localhost:5129",
//                 audience: "http://localhost:5129",
//                 claims: new List<Claim>(),
//                 expires: DateTime.Now.AddMinutes(5),
//                 signingCredentials: signinCredentials
//             );

//             var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

//             return Ok(new AuthenticatedResponse { Token = tokenString });
//         }

//         return Unauthorized();
//     }
// }