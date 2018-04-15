namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackServicios : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PackServicio",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        NumeroVehiculos = c.Int(nullable: false),
                        NumeroOperadores = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.HistorialCargaCombustible", "FechaCreacion", c => c.DateTime(nullable: false));
            AddColumn("dbo.Flota", "PackId", c => c.Int());
            AddColumn("dbo.HistorialMantencion", "FechaCreacion", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Flota", "PackId");
            AddForeignKey("dbo.Flota", "PackId", "dbo.PackServicio", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Flota", "PackId", "dbo.PackServicio");
            DropIndex("dbo.Flota", new[] { "PackId" });
            DropColumn("dbo.HistorialMantencion", "FechaCreacion");
            DropColumn("dbo.Flota", "PackId");
            DropColumn("dbo.HistorialCargaCombustible", "FechaCreacion");
            DropTable("dbo.PackServicio");
        }
    }
}
