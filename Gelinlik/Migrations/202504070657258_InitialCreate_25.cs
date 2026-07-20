namespace Gelinlik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate_25 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DamageReports", "RentalId", "dbo.Rentals");
            DropIndex("dbo.DamageReports", new[] { "RentalId" });
            DropTable("dbo.DamageReports");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.DamageReports", "RentalId");
            AddForeignKey("dbo.DamageReports", "RentalId", "dbo.Rentals", "id", cascadeDelete: true);
        }
    }
}
