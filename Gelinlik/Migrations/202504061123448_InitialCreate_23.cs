namespace Gelinlik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate_23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rentals", "DamageDetails", c => c.String());
            AddColumn("dbo.Rentals", "DamageImagePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rentals", "DamageImagePath");
            DropColumn("dbo.Rentals", "DamageDetails");
        }
    }
}
