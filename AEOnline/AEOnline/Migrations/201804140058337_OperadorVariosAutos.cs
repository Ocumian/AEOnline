namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperadorVariosAutos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Operador", "Auto_Id", "dbo.Auto");
            DropIndex("dbo.Operador", new[] { "Auto_Id" });
            AddColumn("dbo.Auto", "Operador_Id", c => c.Int());
            CreateIndex("dbo.Auto", "Operador_Id");
            AddForeignKey("dbo.Auto", "Operador_Id", "dbo.Operador", "Id");
            DropColumn("dbo.Operador", "Auto_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Operador", "Auto_Id", c => c.Int());
            DropForeignKey("dbo.Auto", "Operador_Id", "dbo.Operador");
            DropIndex("dbo.Auto", new[] { "Operador_Id" });
            DropColumn("dbo.Auto", "Operador_Id");
            CreateIndex("dbo.Operador", "Auto_Id");
            AddForeignKey("dbo.Operador", "Auto_Id", "dbo.Auto", "Id");
        }
    }
}
