using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Seguridad_API.Models;
using System.Linq;

namespace Seguridad_API.Controllers
{
    [Route("api/catalogo")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CatalogoController : ControllerBase
    {
        private readonly RnpdnoContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CatalogoController(RnpdnoContext context, IMapper mapper, ILogger<CatalogoController> logger)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet]
        [Route("{cat:alpha}/{id:int?}")]
        [Authorize(Roles = "superadmin,admin,find")]
        public async Task<ActionResult<List<SelectListItem>>> Get([FromRoute] string cat, int? id = null)
        {
            List<SelectListItem>? list = null;

            try
            {
                if(id != null)
                {
                    if (id.Value > 0)
                    {
                        list = await GetCatalogo(cat, id.Value);
                    }
                    else
                    {
                        _logger.LogError($"Warning {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: Valor id:{id} inválido para cat:{cat}.");
                        return  NotFound($"Valor id:{id} inválido para cat:{cat}.");
                    }
                }
                else
                {
                    list = await GetCatalogo(cat);
                }

                if (list == null)
                {
                    _logger.LogError($"Warning {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: El catálogo {cat} no existe.");
                    return BadRequest($"El catálogo {cat} no existe.");
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}: {ex.Message}");
                throw;
            }
        }


        #region Helpers

        private async Task<List<SelectListItem>> GetCatalogo(string cat)
        {
            List<SelectListItem>? list = null;

            try
            {
                switch (cat)
                {
                    case "estados":
                        list = await _context.cp_estados
                            .Where(x => x.estatus == true)
                            .Select(o => new SelectListItem(o.estado, o.id.ToString()))
                            .ToListAsync();

                        break;
                    case "dependenciaorigen":
                        list = await _context.catdependenciaorigens
                            .Where(x => x.estatus == true)
                            .Select(o => new SelectListItem(o.descripcion, o.iddependenciaorigen.ToString()))
                            .ToListAsync();
                        break;
                    case "modulos":
                        list = await _context.catmodulos
                            .Where(x => x.estatus == 1)
                            .Select(o => new SelectListItem(o.modulo, o.idmodulo.ToString()))
                            .ToListAsync();
                        break;
                    case "roles":
                        list = await _context.seguridad_roles
                            .Where(x => x.estatus == true)
                            .Select(o => new SelectListItem(o.nombre, o.idrol.ToString()))
                            .ToListAsync();
                        break;
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<SelectListItem>> GetCatalogo(string cat, int id)
        {
            List<SelectListItem>? list = null;

            try
            {
                switch (cat)
                {
                    case "dependenciaorigen":
                        list = await _context.catdependenciaorigens
                            .Where(x => x.estatus == true && x.idestado == id)
                            .Select(o => new SelectListItem(o.descripcion, o.iddependenciaorigen.ToString()))
                            .ToListAsync();
                        break;
                    case "roles":
                        list = await _context.seguridad_roles
                            .Where(x => x.estatus == true && x.idmodulo == id)
                            .Select(o => new SelectListItem(o.nombre, o.idrol.ToString()))
                            .ToListAsync();
                        break;
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
