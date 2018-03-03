namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PosicionProcesadaBool : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistorialPosicion", "Procesado", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HistorialPosicion", "Procesado");
        }
    }
}
