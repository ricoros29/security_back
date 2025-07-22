using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

[Table("catmodulo")]
public partial class catmodulo
{
    [Key]
    public byte idmodulo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? modulo { get; set; }

    public byte? estatus { get; set; }
}
