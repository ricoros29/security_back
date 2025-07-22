using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

[Table("catdependenciaorigen")]
public partial class catdependenciaorigen
{
    [Key]
    public short iddependenciaorigen { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? descripcion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? abreviatura { get; set; }

    public int? idestado { get; set; }

    public byte? idmodulo { get; set; }

    public bool? estatuscanalizacion { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? descripciondependencia { get; set; }

    public byte? idtipodependencia { get; set; }

    public bool? estatus { get; set; }
}
