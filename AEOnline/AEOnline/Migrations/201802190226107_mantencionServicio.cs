namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mantencionServicio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alerta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Mensaje = c.String(),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.Auto",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreVehiculo = c.String(nullable: false),
                        Patente = c.String(nullable: false, maxLength: 25),
                        Marca = c.String(maxLength: 25),
                        Modelo = c.String(maxLength: 25),
                        Year = c.Int(nullable: false),
                        KilometrajeActual = c.Int(nullable: false),
                        Latitud = c.Double(nullable: false),
                        Longitud = c.Double(nullable: false),
                        OperadorId = c.Int(),
                        LitrosTotalesConsumidos = c.Single(nullable: false),
                        RendimientoPromedio = c.Single(nullable: false),
                        CostoKilometroPromedio = c.Single(nullable: false),
                        CostoLitroPromedio = c.Single(nullable: false),
                        CostoTotalCombustible = c.Single(nullable: false),
                        CostoTotalMantenimiento = c.Single(nullable: false),
                        Flota_Id = c.Int(),
                        Grupo_Id = c.Int(),
                        TipoVehiculo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .ForeignKey("dbo.GrupoFlota", t => t.Grupo_Id)
                .ForeignKey("dbo.Operador", t => t.OperadorId)
                .ForeignKey("dbo.TipoVehiculo", t => t.TipoVehiculo_Id)
                .Index(t => t.OperadorId)
                .Index(t => t.Flota_Id)
                .Index(t => t.Grupo_Id)
                .Index(t => t.TipoVehiculo_Id);
            
            CreateTable(
                "dbo.HistorialCargaCombustible",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FechaHora = c.DateTime(nullable: false),
                        EstanqueLleno = c.Boolean(nullable: false),
                        CantidadLitros = c.Single(nullable: false),
                        CostoUnitario = c.Int(nullable: false),
                        CostoTotal = c.Int(nullable: false),
                        Kilometraje = c.Int(nullable: false),
                        KilometrosRecorridos = c.Single(nullable: false),
                        CostoPorKilometro = c.Single(nullable: false),
                        Rendimiento = c.Single(nullable: false),
                        Auto_Id = c.Int(),
                        Proveedor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auto", t => t.Auto_Id)
                .ForeignKey("dbo.Proveedor", t => t.Proveedor_Id)
                .Index(t => t.Auto_Id)
                .Index(t => t.Proveedor_Id);
            
            CreateTable(
                "dbo.Proveedor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreComercial = c.String(nullable: false),
                        Telefono = c.String(),
                        Direccion = c.String(),
                        PersonaContacto = c.String(),
                        TelefonoContacto = c.String(),
                        EmailContacto = c.String(),
                        GastoTotalCombustible = c.Int(nullable: false),
                        GastoTotalMantenimiento = c.Int(nullable: false),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.Flota",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 25),
                        UsuarioFlotaId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UsuarioFlota", t => t.UsuarioFlotaId)
                .Index(t => t.Nombre, unique: true, name: "NombreIndex")
                .Index(t => t.UsuarioFlotaId);
            
            CreateTable(
                "dbo.GrupoFlota",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.Operador",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 25),
                        Auto_Id = c.Int(),
                        Usuario_Id = c.Int(),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auto", t => t.Auto_Id)
                .ForeignKey("dbo.Usuario", t => t.Usuario_Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Auto_Id)
                .Index(t => t.Usuario_Id)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 25),
                        Rol = c.Int(nullable: false),
                        Password = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 50),
                        UsuarioFlotaId = c.Int(),
                        OperadorId = c.Int(),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Operador", t => t.OperadorId)
                .ForeignKey("dbo.UsuarioFlota", t => t.UsuarioFlotaId)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Email, unique: true, name: "EmailIndex")
                .Index(t => t.UsuarioFlotaId)
                .Index(t => t.OperadorId)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.UsuarioFlota",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Flota_Id = c.Int(),
                        Usuario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .ForeignKey("dbo.Usuario", t => t.Usuario_Id)
                .Index(t => t.Flota_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "dbo.Recordatorio",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.Servicio",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreServicio = c.String(nullable: false, maxLength: 25),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.TipoVehiculo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tipo = c.String(nullable: false, maxLength: 25),
                        Flota_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flota", t => t.Flota_Id)
                .Index(t => t.Flota_Id);
            
            CreateTable(
                "dbo.HistorialDiario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        Auto_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auto", t => t.Auto_Id)
                .Index(t => t.Auto_Id);
            
            CreateTable(
                "dbo.HistorialEnergia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HoraRegistro = c.DateTime(nullable: false),
                        ValorInicio = c.Single(nullable: false),
                        HoraInicio = c.DateTime(nullable: false),
                        ValorFinal = c.Single(nullable: false),
                        HoraFinal = c.DateTime(nullable: false),
                        ValorMenor = c.Single(nullable: false),
                        HoraMenor = c.DateTime(nullable: false),
                        ValorMayor = c.Single(nullable: false),
                        HoraMayor = c.DateTime(nullable: false),
                        ValorMitad = c.Single(nullable: false),
                        HoraMitad = c.DateTime(nullable: false),
                        HistorialDiario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HistorialDiario", t => t.HistorialDiario_Id)
                .Index(t => t.HistorialDiario_Id);
            
            CreateTable(
                "dbo.HistorialPosicion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FechaHora = c.DateTime(nullable: false),
                        Latitud = c.Double(nullable: false),
                        Longitud = c.Double(nullable: false),
                        Inicio = c.Boolean(nullable: false),
                        HistorialDiario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HistorialDiario", t => t.HistorialDiario_Id)
                .Index(t => t.HistorialDiario_Id);
            
            CreateTable(
                "dbo.HistorialVelocidad",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HoraRegistro = c.DateTime(nullable: false),
                        ValorInicio = c.Single(nullable: false),
                        HoraInicio = c.DateTime(nullable: false),
                        ValorFinal = c.Single(nullable: false),
                        HoraFinal = c.DateTime(nullable: false),
                        ValorMenor = c.Single(nullable: false),
                        HoraMenor = c.DateTime(nullable: false),
                        ValorMayor = c.Single(nullable: false),
                        HoraMayor = c.DateTime(nullable: false),
                        ValorUnCuarto = c.Single(nullable: false),
                        HoraUnCuarto = c.DateTime(nullable: false),
                        ValorMitad = c.Single(nullable: false),
                        HoraMitad = c.DateTime(nullable: false),
                        ValorTresCuartos = c.Single(nullable: false),
                        HoraTresCuartos = c.DateTime(nullable: false),
                        HistorialDiario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HistorialDiario", t => t.HistorialDiario_Id)
                .Index(t => t.HistorialDiario_Id);
            
            CreateTable(
                "dbo.HistorialIncidente",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        Auto_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auto", t => t.Auto_Id)
                .Index(t => t.Auto_Id);
            
            CreateTable(
                "dbo.HistorialMantencion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        Kilometraje = c.Int(nullable: false),
                        TipoDeMantenimiento = c.Int(nullable: false),
                        Costo = c.Int(nullable: false),
                        Auto_Id = c.Int(),
                        Proveedor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auto", t => t.Auto_Id)
                .ForeignKey("dbo.Proveedor", t => t.Proveedor_Id)
                .Index(t => t.Auto_Id)
                .Index(t => t.Proveedor_Id);
            
            CreateTable(
                "dbo.MantencionServicio",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HistorialMantencionId = c.Int(),
                        ServicioId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HistorialMantencion", t => t.HistorialMantencionId)
                .ForeignKey("dbo.Servicio", t => t.ServicioId)
                .Index(t => t.HistorialMantencionId)
                .Index(t => t.ServicioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auto", "TipoVehiculo_Id", "dbo.TipoVehiculo");
            DropForeignKey("dbo.MantencionServicio", "ServicioId", "dbo.Servicio");
            DropForeignKey("dbo.MantencionServicio", "HistorialMantencionId", "dbo.HistorialMantencion");
            DropForeignKey("dbo.HistorialMantencion", "Proveedor_Id", "dbo.Proveedor");
            DropForeignKey("dbo.HistorialMantencion", "Auto_Id", "dbo.Auto");
            DropForeignKey("dbo.Auto", "OperadorId", "dbo.Operador");
            DropForeignKey("dbo.HistorialIncidente", "Auto_Id", "dbo.Auto");
            DropForeignKey("dbo.HistorialDiario", "Auto_Id", "dbo.Auto");
            DropForeignKey("dbo.HistorialVelocidad", "HistorialDiario_Id", "dbo.HistorialDiario");
            DropForeignKey("dbo.HistorialPosicion", "HistorialDiario_Id", "dbo.HistorialDiario");
            DropForeignKey("dbo.HistorialEnergia", "HistorialDiario_Id", "dbo.HistorialDiario");
            DropForeignKey("dbo.Flota", "UsuarioFlotaId", "dbo.UsuarioFlota");
            DropForeignKey("dbo.TipoVehiculo", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Usuario", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Servicio", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Recordatorio", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Proveedor", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Operador", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Operador", "Usuario_Id", "dbo.Usuario");
            DropForeignKey("dbo.Usuario", "UsuarioFlotaId", "dbo.UsuarioFlota");
            DropForeignKey("dbo.UsuarioFlota", "Usuario_Id", "dbo.Usuario");
            DropForeignKey("dbo.UsuarioFlota", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Usuario", "OperadorId", "dbo.Operador");
            DropForeignKey("dbo.Operador", "Auto_Id", "dbo.Auto");
            DropForeignKey("dbo.GrupoFlota", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Auto", "Grupo_Id", "dbo.GrupoFlota");
            DropForeignKey("dbo.Auto", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.Alerta", "Flota_Id", "dbo.Flota");
            DropForeignKey("dbo.HistorialCargaCombustible", "Proveedor_Id", "dbo.Proveedor");
            DropForeignKey("dbo.HistorialCargaCombustible", "Auto_Id", "dbo.Auto");
            DropIndex("dbo.MantencionServicio", new[] { "ServicioId" });
            DropIndex("dbo.MantencionServicio", new[] { "HistorialMantencionId" });
            DropIndex("dbo.HistorialMantencion", new[] { "Proveedor_Id" });
            DropIndex("dbo.HistorialMantencion", new[] { "Auto_Id" });
            DropIndex("dbo.HistorialIncidente", new[] { "Auto_Id" });
            DropIndex("dbo.HistorialVelocidad", new[] { "HistorialDiario_Id" });
            DropIndex("dbo.HistorialPosicion", new[] { "HistorialDiario_Id" });
            DropIndex("dbo.HistorialEnergia", new[] { "HistorialDiario_Id" });
            DropIndex("dbo.HistorialDiario", new[] { "Auto_Id" });
            DropIndex("dbo.TipoVehiculo", new[] { "Flota_Id" });
            DropIndex("dbo.Servicio", new[] { "Flota_Id" });
            DropIndex("dbo.Recordatorio", new[] { "Flota_Id" });
            DropIndex("dbo.UsuarioFlota", new[] { "Usuario_Id" });
            DropIndex("dbo.UsuarioFlota", new[] { "Flota_Id" });
            DropIndex("dbo.Usuario", new[] { "Flota_Id" });
            DropIndex("dbo.Usuario", new[] { "OperadorId" });
            DropIndex("dbo.Usuario", new[] { "UsuarioFlotaId" });
            DropIndex("dbo.Usuario", "EmailIndex");
            DropIndex("dbo.Operador", new[] { "Flota_Id" });
            DropIndex("dbo.Operador", new[] { "Usuario_Id" });
            DropIndex("dbo.Operador", new[] { "Auto_Id" });
            DropIndex("dbo.GrupoFlota", new[] { "Flota_Id" });
            DropIndex("dbo.Flota", new[] { "UsuarioFlotaId" });
            DropIndex("dbo.Flota", "NombreIndex");
            DropIndex("dbo.Proveedor", new[] { "Flota_Id" });
            DropIndex("dbo.HistorialCargaCombustible", new[] { "Proveedor_Id" });
            DropIndex("dbo.HistorialCargaCombustible", new[] { "Auto_Id" });
            DropIndex("dbo.Auto", new[] { "TipoVehiculo_Id" });
            DropIndex("dbo.Auto", new[] { "Grupo_Id" });
            DropIndex("dbo.Auto", new[] { "Flota_Id" });
            DropIndex("dbo.Auto", new[] { "OperadorId" });
            DropIndex("dbo.Alerta", new[] { "Flota_Id" });
            DropTable("dbo.MantencionServicio");
            DropTable("dbo.HistorialMantencion");
            DropTable("dbo.HistorialIncidente");
            DropTable("dbo.HistorialVelocidad");
            DropTable("dbo.HistorialPosicion");
            DropTable("dbo.HistorialEnergia");
            DropTable("dbo.HistorialDiario");
            DropTable("dbo.TipoVehiculo");
            DropTable("dbo.Servicio");
            DropTable("dbo.Recordatorio");
            DropTable("dbo.UsuarioFlota");
            DropTable("dbo.Usuario");
            DropTable("dbo.Operador");
            DropTable("dbo.GrupoFlota");
            DropTable("dbo.Flota");
            DropTable("dbo.Proveedor");
            DropTable("dbo.HistorialCargaCombustible");
            DropTable("dbo.Auto");
            DropTable("dbo.Alerta");
        }
    }
}
