namespace Gelinlik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rentals", "BondPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rentals", "BondPath");
        }
    }
}
