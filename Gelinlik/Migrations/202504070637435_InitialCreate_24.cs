namespace Gelinlik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate_24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rentals", "DamageReporter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rentals", "DamageReporter");
        }
    }
}
