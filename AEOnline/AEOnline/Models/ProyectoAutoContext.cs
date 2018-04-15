using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace AEOnline.Models
{
    public class ProyectoAutoContext : DbContext
    {
        public ProyectoAutoContext() : base("name=ProyectoAutoDB") { }
        //public ProyectoAutoContext() : base("data source=198.38.93.11;initial catalog=stappcl_aedb;user id=stappcl_oscar;password=121212;MultipleActiveResultSets=True;App=EntityFramework") { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Auto> Autos { get; set; }
        public DbSet<Flota> Flotas { get; set; }
        public DbSet<Operador> Operadores { get; set; }
        public DbSet<UsuarioFlota> UsuarioFlotas { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Recordatorio> Recordatorios { get; set; }
        public DbSet<GrupoFlota> GruposFlota { get; set; }
        public DbSet<HistorialCargaCombustible> HistorialesCargaCombustible { get; set; }
        public DbSet<HistorialIncidente> HistorialesIncidentes { get; set; }
        public DbSet<HistorialMantencion> HistorialesMantencion { get; set; }
        public DbSet<MantencionServicio> MantencionServicios { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<TipoVehiculo> TiposVehiculo { get; set; }

        public DbSet<HistorialDiario> HistorialesDiarios { get; set; }

        public DbSet<HistorialVelocidad> HistorialesVelocidad { get; set; }
        public DbSet<HistorialPosicion> HistorialesPosicion { get; set; }
        public DbSet<HistorialEnergia> HistorialEnergia { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasOptional(u => u.Operador)
                .WithMany()
                .HasForeignKey(u => u.OperadorId);

            modelBuilder.Entity<Auto>()
                .HasOptional(a => a.Operador)
                .WithMany()
                .HasForeignKey(a => a.OperadorId);

            modelBuilder.Entity<Usuario>()
                .HasOptional(u => u.UsuarioFlota)
                .WithMany()
                .HasForeignKey(u => u.UsuarioFlotaId);

            modelBuilder.Entity<Flota>()
                .HasOptional(f => f.UsuarioFlota)
                .WithMany()
                .HasForeignKey(f => f.UsuarioFlotaId);

            //modelBuilder.Entity<Ruta>()
            //    .HasRequired(r => r.Linea)
            //    .WithMany(l => l.Rutas)
            //    .HasForeignKey(r => r.LineaId)
            //    .WillCascadeOnDelete(false);


        }

        public System.Data.Entity.DbSet<AEOnline.Models.PackServicio> PackServicios { get; set; }
    }
}