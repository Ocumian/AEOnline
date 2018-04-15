namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GpsOffParche : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistorialPosicion", "GPSOffBool", c => c.Boolean(nullable: false));
            DropColumn("dbo.HistorialPosicion", "GPSOff");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HistorialPosicion", "GPSOff", c => c.DateTime(nullable: false));
            DropColumn("dbo.HistorialPosicion", "GPSOffBool");
        }
    }
}
