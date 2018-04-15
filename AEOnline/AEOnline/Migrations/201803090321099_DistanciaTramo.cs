namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DistanciaTramo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistorialPosicion", "MetrosTramo", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HistorialPosicion", "MetrosTramo");
        }
    }
}
