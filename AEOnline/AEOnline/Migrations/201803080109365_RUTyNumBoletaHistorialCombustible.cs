namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RUTyNumBoletaHistorialCombustible : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistorialCargaCombustible", "RutEstacion", c => c.String());
            AddColumn("dbo.HistorialCargaCombustible", "NumeroDeBoleta", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HistorialCargaCombustible", "NumeroDeBoleta");
            DropColumn("dbo.HistorialCargaCombustible", "RutEstacion");
        }
    }
}
