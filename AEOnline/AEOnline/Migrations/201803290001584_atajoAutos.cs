namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atajoAutos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auto", "Atajo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auto", "Atajo");
        }
    }
}
