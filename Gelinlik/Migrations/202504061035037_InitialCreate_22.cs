namespace Gelinlik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate_22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DamageReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RentalId = c.Int(nullable: false),
                        DamageDetails = c.String(),
                        ImagePath = c.String(),
                        ReportDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rentals", t => t.RentalId, cascadeDelete: true)
                .Index(t => t.RentalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DamageReports", "RentalId", "dbo.Rentals");
            DropIndex("dbo.DamageReports", new[] { "RentalId" });
            DropTable("dbo.DamageReports");
        }
    }
}
