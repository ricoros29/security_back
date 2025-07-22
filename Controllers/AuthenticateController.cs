using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Seguridad_API.DTO.Security;
using Seguridad_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/authenticate")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthenticateController : ControllerBase
    {
        private readonly RnpdnoContext _rnpdnoContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthenticateController(RnpdnoContext rnpdnoContext, IConfiguration configuration, ILogger<AuthenticateController> logger)
        {
            this._rnpdnoContext = rnpdnoContext;
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<ResultLoginDTO> Login(CredentialsDTO credentials)
        {
            if (credentials is null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var passHash = ComputeSha256Hash(credentials.Password);

                var userLogin = _users.Find(x => x.Username == credentials.UserName && x.Hash == passHash);

                if (userLogin == null)
                {
                    return BadRequest("Usuario o contraseña inválida.");
                }
                else
                {
                    return GenerarToken(userLogin);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: {ex.Message}");
                throw;
            }
        }

        #region Helpers

        private ResultLoginDTO GenerarToken(UserLogin userLogin)
        {
            try
            {
                var claims = new List<Claim>()
                {
                    new(ClaimTypes.Sid, userLogin.IdUsuario),
                    new(ClaimTypes.NameIdentifier, userLogin.Username)
                };

                //Obtener los roles de la base de datos
                var claimsDB = GetClaimsRol(userLogin.Roles);
                claims.AddRange(claimsDB);

                var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtSecretKey"] ?? throw new InvalidOperationException("jwtSecretKey not found")));
                var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

                var expiracion = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["jwtExpiredMinutes"] ?? throw new InvalidOperationException("jwtExpiredMinutes not found")));

                var iss = _configuration["jwtIssuer"] ?? throw new InvalidOperationException("jwtIssuer not found");

                var aud = _configuration["jwtAudience"] ?? throw new InvalidOperationException("jwtAudience not found");

                var securityToken = new JwtSecurityToken(issuer: iss, audience: aud, claims: claims,
                    expires: expiracion, signingCredentials: creds);

                return new ResultLoginDTO()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                    Expires = expiracion,
                    Username = userLogin.Username,
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: {ex.Message}");
                throw;
            }
        }

        private void EncriptarPassword(ref string Pwd)
        {
            try
            {
                var shaM1 = new SHA1Managed();

                byte[] data1 = Encoding.ASCII.GetBytes(Pwd);
                byte[] hash = shaM1.ComputeHash(data1);

                Pwd = Encoding.ASCII.GetString(hash);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: {ex.Message}");
                throw;
            }
        }

        private List<Claim> GetClaimsRol(List<string> roles)
        {
            try
            {
                var Claims = new List<Claim>();

                roles.ForEach(x => Claims.Add(new(ClaimTypes.Role, x)));

                return Claims;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: {ex.Message}");
                throw;
            }
        }

        private bool VerifyMD5Hash(string password, string hash)
        {
            string inputHash = ComputeMD5Hash(password);
            return StringComparer.OrdinalIgnoreCase.Compare(inputHash, hash) == 0;
        }

        private string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert hash bytes to hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash as a byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static readonly List<UserLogin> _users = new List<UserLogin>
        {
        new UserLogin() {
            IdUsuario= "4AB4742F-5AB1-4BCC-9CE6-27B2DF1D73D2",
            Username = "superadmin",
            Password ="p6%aYh#0R7R@TkfeMA8.UW?L",
            Hash = "a956de955a3ab4b6f69ecab125a64ed05551fcbcba1921c5ae64649b2ad25e50",
            Roles = ["admin"]
        },
        new UserLogin() {
            IdUsuario="3F69F3BB-5D6D-416C-923F-E07C483F1704",
            Username = "admin",
            Password ="V3r0n1k4.",
            Hash = "d1e11658b660daf0179191557a95280c82002f3df70f6766e614e7d8da35229b",
            Roles = ["create","find","edit","delete","details"]
        },
        new UserLogin() {
            IdUsuario="8C3E441A-0EE4-43DF-BC64-2253A0396C80",
            Username = "search",
            Password ="c0nsult4.",
            Hash = "fcd04d9a7dfae7fe9125c8b3dc678659917c7362d9eaf470730de7ff94ddc366",
            Roles = ["find", "details"]
        },
        new UserLogin() {
            IdUsuario="1F5E277C-B56C-4A39-9616-AD7BCAF18B43",
            Username = "test",
            Password ="12345",
            Hash = "5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5",
            Roles = ["create","find","edit","delete","details"]
        }, new UserLogin() {
            IdUsuario="56E7E953-9025-44B0-8FB5-2F0846CD3DD5",
            Username = "desarrollo",
            Password ="12345",
            Hash = "5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5",
            Roles = ["admin"]
        }
        };

        #endregion
    }

    public class UserLogin
    {
        public required string IdUsuario { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Hash { get; set; }
        public required List<string> Roles { get; set; }

    }
}


