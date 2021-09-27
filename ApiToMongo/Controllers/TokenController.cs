using ApiToMongo.Helpers;
using ApiToMongo.Models;
using DataLibrary;
using DataLibrary.Entity;
using DataLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiToMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IDbContext _dbContex;
        private readonly AppSettings _appSettings;
        public TokenController(IDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContex = dbContext;
            _appSettings = appSettings.Value;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] TokenRequestModel model) // granttype = "refresh_token"
        {
            // We will return Generic 500 HTTP Server Status Error
            // If we receive an invalid payload
            if (model == null)
            {
                return new StatusCodeResult(500);
            }

            switch (model.GrantType)
            {
                case "password":
                    return await GenerateNewToken(model);
                case "refresh_token":
                    return await RefreshToken(model);
                default:
                    // not supported - return a HTTP 401 (Unauthorized)
                    return new UnauthorizedResult();
            }

        }

        // Method to Create New JWT and Refresh Token
        private async Task<IActionResult> GenerateNewToken(TokenRequestModel model)
        {
            // check if there's an user with the given loginid
            var entity = (_dbContex.LoadContext("Users") as UserEntity);
            var filter = entity.GetUserDetails<UserDetails>(model.UserName);
            var record = await _dbContex.LoadRecordByFilterAsync("Users", filter);
            var user = record.FirstOrDefault();

            // Validate credentials
            if (user != null && user.Password == model.Password)
            {
                // username & password matches: create the refresh token
                var newRtoken = CreateRefreshToken(_appSettings.ClientId, user.Id);

                // first we delete any existing old refreshtokens
                var tokenFilter = (_dbContex.LoadContext("Tokens") as TokenEntity).GetTokenDetails<TokenModel>(user.Id); 
                var oldrTokens= await _dbContex.LoadRecordByFilterAsync("Tokens", tokenFilter);

                //if (oldrTokens != null)
                //{
                //    foreach (var oldrt in oldrTokens)
                //    {
                //        _db.Tokens.Remove(oldrt);
                //    }

                //}
                
                if(oldrTokens.Count>0)
                {
                    newRtoken.Id = oldrTokens.FirstOrDefault().Id;
                }

                // Add new refresh token to Database
                //_db.Tokens.Add(newRtoken);
                await _dbContex.UpsertRecordAsync<TokenModel>("Tokens", newRtoken.Id, newRtoken);

                // Create & Return the access token which contains JWT and Refresh Token

                var accessToken = await CreateAccessToken(user, newRtoken.Value);


                return Ok(new { authToken = accessToken });

            }

            ModelState.AddModelError("", "Username/Password was not Found");
            return Unauthorized(new { LoginError = "Please Check the Login Credentials - Ivalid Username/Password was entered" });


        }

        // Create access Tokenm
        private async Task<TokenResponseModel> CreateAccessToken(UserDetails user, string refreshToken)
        {

            double tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            var roles = new List<String> { "User" };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Firstname+" "+user.Lastname),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),

                     }),

                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.Site,
                Audience = _appSettings.Audience,
                Expires = DateTime.Now.AddMinutes(tokenExpiryTime)
            };

            // Generate token

            var newtoken = tokenHandler.CreateToken(tokenDescriptor);

            var encodedToken = tokenHandler.WriteToken(newtoken);

            return new TokenResponseModel()
            {
                token = encodedToken,
                expiration = newtoken.ValidTo,
                refresh_token = refreshToken,
                roles = roles.FirstOrDefault(),
                username = user.Firstname + " " + user.Lastname
            };


        }


        private TokenModel CreateRefreshToken(string clientId, Guid userId)
        {
            return new TokenModel()
            {
                Id=new Guid(),
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(90)
            };
        }



        // Method to Refresh JWT and Refresh Token
        private async Task<IActionResult> RefreshToken(TokenRequestModel model)
        {
            try
            {
                // check if the received refreshToken exists for the given clientId
                var tokenFilter = (_dbContex.LoadContext("Tokens") as TokenEntity).GetTokenByClientAndValue<TokenModel>(_appSettings.ClientId,model.RefreshToken.ToString());
                var oldrTokens = await _dbContex.LoadRecordByFilterAsync("Tokens", tokenFilter);

                if (oldrTokens.Count==0)
                {
                    // refresh token not found or invalid (or invalid clientId)
                    return new UnauthorizedResult();
                }
                var rt = oldrTokens.FirstOrDefault();
                // check if refresh token is expired
                if (rt.ExpiryTime < DateTime.UtcNow)
                {
                    return new UnauthorizedResult();
                }

                // check if there's an user with the refresh token's userId
                var user = await _dbContex.LoadRecordByIdAsync<UserDetails>("Users", rt.UserId); 


                if (user == null)
                {
                    // UserId not found or invalid
                    return new UnauthorizedResult();
                }

                // generate a new refresh token 

                var rtNew = CreateRefreshToken(rt.ClientId, rt.UserId);

                // invalidate the old refresh token (by deleting it)
                // add the new refresh token
                if (rt != null)
                {
                    rtNew.Id = rt.Id;
                }

                await _dbContex.UpsertRecordAsync<TokenModel>("Tokens", rtNew.Id, rtNew);

                // 

                var response = await CreateAccessToken(user, rtNew.Value);

                return Ok(new { authToken = response });

            }
            catch (Exception ex)
            {

                return new UnauthorizedResult();
            }
        }

    }
}
