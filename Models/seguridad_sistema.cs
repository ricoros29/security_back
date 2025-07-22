using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

[Table("seguridad_sistema")]
public partial class seguridad_sistema
{
    [Key]
    public int idsistema { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    public bool estatus { get; set; }

    [InverseProperty("idsistemaNavigation")]
    public virtual ICollection<seguridad_usuario_sistema> seguridad_usuario_sistemas { get; set; } = new List<seguridad_usuario_sistema>();
}
