namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HistorialPosicionUpdateGPsoffLocalidad : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistorialPosicion", "NombreCalle", c => c.String());
            AddColumn("dbo.HistorialPosicion", "NombreLocalidad", c => c.String());
            AddColumn("dbo.HistorialPosicion", "GPSOff", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HistorialPosicion", "GPSOff");
            DropColumn("dbo.HistorialPosicion", "NombreLocalidad");
            DropColumn("dbo.HistorialPosicion", "NombreCalle");
        }
    }
}
