using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

[Index("noempleado", Name = "NoEmpleado_NonDup", IsUnique = true)]
[Index("cuenta", Name = "UQ__segurida__682E1D9C253173CA", IsUnique = true)]
public partial class seguridad_usuario
{
    [Key]
    public int idusuario { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? nombre { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? apellidopaterno { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? apellidomaterno { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? cuenta { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? password { get; set; }

    public bool? accesosinrestriccion { get; set; }

    public bool? restringidoenhorario { get; set; }

    public bool? restringidoenip { get; set; }

    public int? idestado { get; set; }

    public short? iddependenciaorigen { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? noempleado { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? rfc { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? cargo { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? unidadadministrativa { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? correoelectronico { get; set; }

    public byte? idmodulo { get; set; }

    public bool sesionactiva { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechasesion { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string? ipsesion { get; set; }

    public bool? estatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fecharegistro { get; set; }

    [InverseProperty("idusuarioNavigation")]
    public virtual seguridad_usuario_role? seguridad_usuario_role { get; set; }

    [InverseProperty("idusuarioNavigation")]
    public virtual ICollection<seguridad_usuario_sistema> seguridad_usuario_sistemas { get; set; } = new List<seguridad_usuario_sistema>();
}
