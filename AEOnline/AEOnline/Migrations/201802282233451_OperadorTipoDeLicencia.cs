namespace AEOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperadorTipoDeLicencia : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Operador", "TipoLicencia", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Operador", "TipoLicencia");
        }
    }
}
