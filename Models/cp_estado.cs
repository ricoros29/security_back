using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

public partial class cp_estado
{
    [Key]
    public int id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string estado { get; set; } = null!;

    [StringLength(4)]
    [Unicode(false)]
    public string abreviatura { get; set; } = null!;

    public bool estatus { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string idrenapo { get; set; } = null!;

    [StringLength(25)]
    [Unicode(false)]
    public string latitud { get; set; } = null!;

    [StringLength(25)]
    [Unicode(false)]
    public string longitud { get; set; } = null!;

    [StringLength(8)]
    [Unicode(false)]
    public string? clavegrafica { get; set; }

    public bool? estatusrenapo { get; set; }
}
