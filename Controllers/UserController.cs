using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Seguridad_API.DTO;
using Seguridad_API.DTO.Users;
using Seguridad_API.Interfaces;
using Seguridad_API.Models;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Seguridad_API.Server.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly RnpdnoContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserController(RnpdnoContext context, IMapper mapper, ILogger<UserController> logger)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "superadmin,admin,create")]
        public async Task<ActionResult<UserDTO>> Post(UserDTO? userDTO)
        {
            if (userDTO is null || !ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }

            try
            {
                var Existe = await ExistUserAccount(userDTO.Cuenta);

                if (Existe)
                {
                    return BadRequest($"No se pudo registrar. Ya existe un usuario con la cuenta {userDTO.Cuenta!.ToUpper()}");
                }

                //Clonar
                var newUserDTO = (UserDTO)userDTO.Clone();

                //Convertir a mayusculas
                newUserDTO.Nombre = newUserDTO.Nombre.Trim().ToUpper();
                newUserDTO.ApellidoPaterno = newUserDTO.ApellidoPaterno.Trim().ToUpper();
                newUserDTO.ApellidoMaterno = newUserDTO.ApellidoMaterno.Trim().ToUpper();
                newUserDTO.Cargo = newUserDTO.Cargo.Trim().ToUpper();
                newUserDTO.Rfc = newUserDTO.Rfc.Trim().ToUpper();
                newUserDTO.UnidadAdministrativa = newUserDTO.UnidadAdministrativa.Trim().ToUpper();
                newUserDTO.Cuenta = newUserDTO.Cuenta.Trim().ToUpper();

                if(newUserDTO.IdEstado == 33 && newUserDTO.IdDependenciaOrigen == 42)
                {
                    //Se sustituye por entidad CDMX porque asi lo guardaban antes en seguridad.
                    newUserDTO.IdEstado = 9;
                }

                //Mapear
                var user = _mapper.Map<seguridad_usuario>(newUserDTO);

                //Obtener IdUsuario
                var idUser = await GetNextIdUser();

                //Encriptar Password
                var passEncript = newUserDTO.Password;
                EncriptarPassword(ref passEncript);

                var FechaActual = DateTime.Now;

                //Asignar nuevos valores a insertar y default
                user.idusuario = idUser;
                user.noempleado = idUser.ToString();
                user.password = passEncript;
                user.accesosinrestriccion = true;
                user.restringidoenhorario = false;
                user.restringidoenip = false;
                user.sesionactiva = false;
                user.estatus = true;
                user.fecharegistro = FechaActual;

                using (var transacction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.seguridad_usuarios.Add(user);
                        await _context.SaveChangesAsync();

                        var role = new seguridad_usuario_role();
                        role.idusuario = idUser;
                        role.idrol = newUserDTO.IdRol.Value;
                        role.fechacpatura = FechaActual;
                        role.estatus = true;

                        _context.seguridad_usuario_roles.Add(role);
                        await _context.SaveChangesAsync();

                        await transacction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        if (transacction != null)
                        {
                            transacction.Rollback();
                        }

                        throw;
                    }
                }


                //var userCreated = _mapper.Map<UserDTO>(newUserDTO);
                //userCreated.Cuenta = userDTO.Cuenta;
                //userCreated.Password = userDTO.Password;

                var locationUri = $"{Request.Host}/api/user/{idUser}";

                return Created(locationUri, newUserDTO);

            }
            catch (Exception ex)
            {
                WriteLog(ex);
                throw;
            }
        }

        [HttpPut]
        [Route("Edit/{id}")]
        [Authorize(Roles = "superadmin,admin,edit")]
        public async Task<ActionResult<bool>> Put([FromRoute] int id, [FromBody] UserFindDTO userDTO)
        {
            if (userDTO is null || !ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }
            else if (id != userDTO.IdUsuario)
            {
                return BadRequest("Bad Request: Not valid id.");
            }

            try
            {
                var user = _context.seguridad_usuarios.Find(userDTO.IdUsuario);
                var role = _context.seguridad_usuario_roles.Where(x => x.idusuario == userDTO.IdUsuario).FirstOrDefault();

                if (user == null || role == null)
                {
                    return NotFound("Usuario o rol inválido.");
                }

                //Asignar nuevos valores
                //user.idusuario = userDTO.IdUsuario;
                user.nombre = userDTO.Nombre?.Trim().ToUpper();
                user.apellidopaterno = userDTO.ApellidoPaterno?.Trim().ToUpper();
                user.apellidomaterno = userDTO.ApellidoMaterno?.Trim().ToUpper();
                user.rfc = userDTO.Rfc?.Trim().ToUpper();
                //user.idestado = userDTO.IdEstado;
                //user.iddependenciaorigen = userDTO.IdDependenciaOrigen;
                //user.idmodulo = userDTO.IdModulo;
                user.correoelectronico = userDTO.CorreoElectronico;
                user.cargo = userDTO.Cargo?.Trim().ToUpper();
                //user.unidadadministrativa = userDTO.UnidadAdministrativa?.Trim().ToUpper();
                user.estatus = userDTO.IdEstatus;

                if (userDTO.IdEstatus == false)
                {
                    user.password = $"BAJA_{DateTime.Now.ToString("dd-MM-yyyy")}";
                }

                using (var transacction = _context.Database.BeginTransaction())
                {
                    var uptUser = 0;
                    var uptRol = 0;

                    try
                    {
                        _context.Entry<seguridad_usuario>(user).State = EntityState.Modified;
                        uptUser = await _context.SaveChangesAsync();

                        if (role != null)
                        {
                            //role.idusuario = userDTO.IdUsuario;
                            role.idrol = userDTO.IdRol.Value;

                            if (userDTO.IdEstatus == false)
                            {
                                role.estatus = false;
                            }

                            _context.Entry<seguridad_usuario_role>(role).State = EntityState.Modified;
                            uptRol = await _context.SaveChangesAsync();
                        }

                        await transacction.CommitAsync();

                        if (uptUser > 0 || uptRol > 0)
                            return Ok(true);
                        else
                            return Ok(false);
                    }
                    catch (Exception)
                    {
                        if (transacction != null)
                        {
                            transacction.Rollback();
                        }

                        throw;
                    }
                }

            }
            catch (Exception ex)
            {
                WriteLog(ex);
                throw;
            }
        }

        [HttpPut]
        [Route("Reset/{id}")]
        [Authorize(Roles = "superadmin,admin,reset")]
        public async Task<ActionResult<bool>> Reset([FromRoute] int id, [FromBody] ResetDTO resetDTO)
        {
            if (resetDTO is null || !ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }
            else if (id != resetDTO.IdUsuario)
            {
                return BadRequest("Bad Request: Not valid id.");
            }
            else if (resetDTO.Password != resetDTO.PasswordConfirm)
            {
                return BadRequest("La contraseña y su confirmación no coinciden.");
            }

            try
            {
                var user = _context.seguridad_usuarios.Find(resetDTO.IdUsuario);

                if (user == null)
                {
                    return NotFound("Usuario inválido.");
                }

                var passEncript = resetDTO.Password;
                EncriptarPassword(ref passEncript);

                //Asignar nuevos valores
                user.password = passEncript;

                _context.Entry<seguridad_usuario>(user).State = EntityState.Modified;
                var uptUser = await _context.SaveChangesAsync();

                if (uptUser > 0)
                    return Ok(true);
                else
                    return Ok(false);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserById/{id:int}")]
        [Authorize(Roles = "superadmin,admin,find")]
        public async Task<ActionResult<UserFindDTO>> Get([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                var user = await GetUserById(id);

                if (user == null)
                {
                    return NotFound("Invalid id.");
                }
                else
                {
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("FindUsers/{texto}")]
        [Authorize(Roles = "superadmin,admin,find")]
        public async Task<ActionResult<List<UserFindDTO>>> FindUsers([FromRoute] string texto)
        {
            if (string.IsNullOrEmpty(texto) || texto.Length < 3)
            {
                return BadRequest("Debe indicar al menos 3 caracteres del Nombre completo o Cuenta de usuario.");
            }

            try
            {
                var users = await GetUsersByName(texto.ToUpper());

                if (users == null || users.Count == 0)
                {
                    users = await GetUsersByAccount(texto.ToUpper());

                    if (users == null || users.Count == 0)
                    {
                        return NotFound($"No hay coincidencias con {texto.ToUpper()}.");
                    }
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("ExistAccount/{account}")]
        [Authorize(Roles = "superadmin,admin,find")]
        public async Task<ActionResult<bool>> ExistAccount([FromRoute] string account)
        {
            if (string.IsNullOrEmpty(account) || account.Length < 5)
            {
                return BadRequest("Debe indicar al menos 5 caracteres de la Cuenta de usuario.");
            }

            try
            {
                bool ExisteCuenta = await ExistUserAccount(account);

                return Ok(ExisteCuenta);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                throw;
            }
        }

        #region Helpers

        private async Task<List<UserFindDTO>?> GetUsersByName(string texto)
        {
            try
            {
                var NombreCompleto = texto;

                var query = $"SELECT su.idusuario, su.nombre, su.apellidopaterno, su.apellidomaterno, su.cuenta, su.iddependenciaorigen, su.idestado, su.rfc, su.correoelectronico, su.noempleado, ce.estado, cd.descripciondependencia dependenciaorigen, sur.idrol, sro.nombre as rol, su.cargo, su.unidadadministrativa, su.idmodulo, cm.modulo, su.estatus as idestatus, case(su.estatus) when 1 then 'ACTIVO' when 0 then 'INACTIVO' END as estatus " +
                    $"FROM seguridad_usuarios su " +
                    $"LEFT JOIN cp_estados ce on su.idestado = ce.id " +
                    $"LEFT JOIN catdependenciaorigen cd on su.iddependenciaorigen = cd.iddependenciaorigen " +
                    $"LEFT JOIN seguridad_usuario_roles sur on su.idusuario = sur.idusuario " +
                    $"LEFT JOIN seguridad_roles sro on sur.idrol = sro.idrol " +
                    $"LEFT JOIN catmodulo cm on su.idmodulo = cm.idmodulo " +
                    $"WHERE (su.nombre + ' ' + su.apellidopaterno + ' ' + su.apellidomaterno) like '%{NombreCompleto}%'";

                //if (!string.IsNullOrEmpty(NombreCompleto))
                //{
                //    query += $" AND (su.nombre + ' ' + su.apellidopaterno + ' ' + su.apellidomaterno) like '%{NombreCompleto}%'";
                //}

                var users = await _context.Database.SqlQuery<UserFindDTO>(FormattableStringFactory.Create(query))
                  .OrderByDescending(b => b.Nombre)
                  .ToListAsync();

                return users;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<UserFindDTO>?> GetUsersByAccount(string texto)
        {
            try
            {
                var Cuenta = texto;

                var query = $"SELECT su.idusuario, su.nombre, su.apellidopaterno, su.apellidomaterno, su.cuenta, su.iddependenciaorigen, su.idestado, su.rfc, su.correoelectronico, su.noempleado, ce.estado, cd.descripciondependencia dependenciaorigen, sur.idrol, sro.nombre as rol, su.cargo, su.unidadadministrativa, su.idmodulo, cm.modulo, su.estatus as idestatus, case(su.estatus) when 1 then 'ACTIVO' when 0 then 'INACTIVO' END as estatus " +
                    $"FROM seguridad_usuarios su " +
                    $"LEFT JOIN cp_estados ce on su.idestado = ce.id " +
                    $"LEFT JOIN catdependenciaorigen cd on su.iddependenciaorigen = cd.iddependenciaorigen " +
                    $"LEFT JOIN seguridad_usuario_roles sur on su.idusuario = sur.idusuario " +
                    $"LEFT JOIN seguridad_roles sro on sur.idrol = sro.idrol " +
                    $"LEFT JOIN catmodulo cm on su.idmodulo = cm.idmodulo " +
                    $"WHERE su.cuenta like '%{Cuenta}%' ";

                //if (!string.IsNullOrEmpty(Cuenta))
                //{
                //    query += $" AND ";
                //}

                var users = await _context.Database.SqlQuery<UserFindDTO>(FormattableStringFactory.Create(query))
                  .OrderByDescending(b => b.Nombre)
                  .ToListAsync();

                return users;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<UserFindDTO?> GetUserById(int id)
        {
            try
            {
                var user = await _context.Database.SqlQuery<UserFindDTO?>(
                 $"SELECT su.idusuario, su.nombre, su.apellidopaterno, su.apellidomaterno, su.cuenta, su.iddependenciaorigen, su.idestado, su.rfc, su.correoelectronico, su.noempleado, ce.estado, cd.descripciondependencia dependenciaorigen, sur.idrol, sro.nombre as rol, su.cargo, su.unidadadministrativa, su.idmodulo, cm.modulo, su.estatus as idestatus, case(su.estatus) when 1 then 'ACTIVO' when 0 then 'INACTIVO' END as estatus  FROM seguridad_usuarios su LEFT JOIN cp_estados ce on su.idestado = ce.id LEFT JOIN catdependenciaorigen cd on su.iddependenciaorigen = cd.iddependenciaorigen LEFT JOIN seguridad_usuario_roles sur on su.idusuario = sur.idusuario LEFT JOIN seguridad_roles sro on sur.idrol = sro.idrol LEFT JOIN catmodulo cm on su.idmodulo = cm.idmodulo WHERE su.idusuario = {id}"
                 ).FirstOrDefaultAsync();

                return user;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<int> GetNextIdUser()
        {
            try
            {
                var idUser = await _context.seguridad_usuarios.Select(x => x.idusuario).MaxAsync();

                idUser += 1;

                return idUser;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<bool> ExistUserAccount(string account)
        {
            try
            {
                if(string.IsNullOrEmpty(account))
                    throw new ArgumentNullException("account");

                bool ExisteCuenta = await _context.seguridad_usuarios.AnyAsync(x => x.cuenta!.ToUpper() == account.ToUpper());

                return ExisteCuenta;
            }
            catch (Exception)
            {

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
            catch (Exception)
            {
                throw;
            }
        }

        private string RemoverAcentos(string texto)
        {
            var conAcentos = "áéíóúÁÉÍÓÚñÑ";
            var sinAcentos = "aeiouAEIOUnN";

            if (string.IsNullOrEmpty(texto))
            {
                return string.Empty;
            }

            var textoSinAcentos = new StringBuilder(texto.Length);

            foreach (char c in texto)
            {
                int index = conAcentos.IndexOf(c);

                if (index >= 0)
                {
                    textoSinAcentos.Append(sinAcentos[index]);
                }
                else
                {
                    textoSinAcentos.Append(c);
                }
            }

            return textoSinAcentos.ToString();
        }

        private void WriteLog(Exception ex)
        {
            _logger.LogError($"Error {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: {(ex.InnerException != null ? ex.InnerException : ex.Message)}");
        }

        #endregion
    }
}
