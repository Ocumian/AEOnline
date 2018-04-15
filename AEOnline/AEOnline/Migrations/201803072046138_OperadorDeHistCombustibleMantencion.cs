namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperadorDeHistCombustibleMantencion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistorialCargaCombustible", "Operador_Id", c => c.Int());
            AddColumn("dbo.HistorialMantencion", "Operador_Id", c => c.Int());
            CreateIndex("dbo.HistorialCargaCombustible", "Operador_Id");
            CreateIndex("dbo.HistorialMantencion", "Operador_Id");
            AddForeignKey("dbo.HistorialCargaCombustible", "Operador_Id", "dbo.Operador", "Id");
            AddForeignKey("dbo.HistorialMantencion", "Operador_Id", "dbo.Operador", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HistorialMantencion", "Operador_Id", "dbo.Operador");
            DropForeignKey("dbo.HistorialCargaCombustible", "Operador_Id", "dbo.Operador");
            DropIndex("dbo.HistorialMantencion", new[] { "Operador_Id" });
            DropIndex("dbo.HistorialCargaCombustible", new[] { "Operador_Id" });
            DropColumn("dbo.HistorialMantencion", "Operador_Id");
            DropColumn("dbo.HistorialCargaCombustible", "Operador_Id");
        }
    }
}
