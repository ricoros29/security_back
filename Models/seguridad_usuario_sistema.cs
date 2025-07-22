using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

[PrimaryKey("idusuario", "idsistema")]
[Table("seguridad_usuario_sistema")]
public partial class seguridad_usuario_sistema
{
    [Key]
    public int idusuario { get; set; }

    [Key]
    public int idsistema { get; set; }

    public bool? estatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechacpatura { get; set; }

    [ForeignKey("idsistema")]
    [InverseProperty("seguridad_usuario_sistemas")]
    public virtual seguridad_sistema idsistemaNavigation { get; set; } = null!;

    [ForeignKey("idusuario")]
    [InverseProperty("seguridad_usuario_sistemas")]
    public virtual seguridad_usuario idusuarioNavigation { get; set; } = null!;
}
