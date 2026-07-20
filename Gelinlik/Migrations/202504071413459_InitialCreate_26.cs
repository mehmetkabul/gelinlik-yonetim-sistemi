namespace Gelinlik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate_26 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Action = c.String(),
                        PerformedBy = c.String(),
                        Date = c.DateTime(nullable: false),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Activities");
        }
    }
}
