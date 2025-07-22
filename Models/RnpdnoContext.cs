using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Seguridad_API.Models;

public partial class RnpdnoContext : DbContext
{
    public RnpdnoContext(DbContextOptions<RnpdnoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<catdependenciaorigen> catdependenciaorigens { get; set; }

    public virtual DbSet<catmodulo> catmodulos { get; set; }

    public virtual DbSet<cp_estado> cp_estados { get; set; }

    public virtual DbSet<seguridad_role> seguridad_roles { get; set; }

    public virtual DbSet<seguridad_sistema> seguridad_sistemas { get; set; }

    public virtual DbSet<seguridad_usuario> seguridad_usuarios { get; set; }

    public virtual DbSet<seguridad_usuario_role> seguridad_usuario_roles { get; set; }

    public virtual DbSet<seguridad_usuario_sistema> seguridad_usuario_sistemas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<catdependenciaorigen>(entity =>
        {
            entity.HasKey(e => e.iddependenciaorigen).HasName("PK_catorigendependencia");

            entity.Property(e => e.iddependenciaorigen).ValueGeneratedNever();
        });

        modelBuilder.Entity<cp_estado>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK_estados");

            entity.Property(e => e.id).ValueGeneratedNever();
        });

        modelBuilder.Entity<seguridad_role>(entity =>
        {
            entity.HasKey(e => e.idrol).HasName("PK_Roles");
        });

        modelBuilder.Entity<seguridad_sistema>(entity =>
        {
            entity.HasKey(e => e.idsistema).HasName("PK_Sistema");
        });

        modelBuilder.Entity<seguridad_usuario>(entity =>
        {
            entity.HasKey(e => e.idusuario).HasName("PK_usuarios");

            entity.Property(e => e.idusuario).ValueGeneratedNever();
            entity.Property(e => e.accesosinrestriccion).HasDefaultValue(false);
            entity.Property(e => e.fecharegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.restringidoenhorario).HasDefaultValue(false);
            entity.Property(e => e.restringidoenip).HasDefaultValue(false);
        });

        modelBuilder.Entity<seguridad_usuario_role>(entity =>
        {
            entity.Property(e => e.idusuario).ValueGeneratedNever();
            entity.Property(e => e.fechacpatura).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.idrolNavigation).WithMany(p => p.seguridad_usuario_roles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_seguridad_usuario_roles_seguridad_roles");

            entity.HasOne(d => d.idusuarioNavigation).WithOne(p => p.seguridad_usuario_role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_seguridad_usuario_roles_seguridad_usuarios");
        });

        modelBuilder.Entity<seguridad_usuario_sistema>(entity =>
        {
            entity.HasOne(d => d.idsistemaNavigation).WithMany(p => p.seguridad_usuario_sistemas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_seguridad_usuario_sistema_seguridad_sistema");

            entity.HasOne(d => d.idusuarioNavigation).WithMany(p => p.seguridad_usuario_sistemas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_seguridad_usuario_sistema_seguridad_usuarios");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
