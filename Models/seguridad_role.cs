using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

public partial class seguridad_role
{
    [Key]
    public int idrol { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    public byte prioridad { get; set; }

    public bool estatus { get; set; }

    public byte? idmodulo { get; set; }

    [InverseProperty("idrolNavigation")]
    public virtual ICollection<seguridad_usuario_role> seguridad_usuario_roles { get; set; } = new List<seguridad_usuario_role>();
}
