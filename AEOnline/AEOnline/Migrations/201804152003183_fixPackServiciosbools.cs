namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixPackServiciosbools : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PackServicio", "ModuloCombustible", c => c.Boolean(nullable: false));
            AddColumn("dbo.PackServicio", "ModuloMantencion", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PackServicio", "ModuloMantencion");
            DropColumn("dbo.PackServicio", "ModuloCombustible");
        }
    }
}
