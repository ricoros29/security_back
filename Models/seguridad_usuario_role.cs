using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

public partial class seguridad_usuario_role
{
    [Key]
    public int idusuario { get; set; }

    public int idrol { get; set; }

    public bool? estatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechacpatura { get; set; }

    [ForeignKey("idrol")]
    [InverseProperty("seguridad_usuario_roles")]
    public virtual seguridad_role idrolNavigation { get; set; } = null!;

    [ForeignKey("idusuario")]
    [InverseProperty("seguridad_usuario_role")]
    public virtual seguridad_usuario idusuarioNavigation { get; set; } = null!;
}
