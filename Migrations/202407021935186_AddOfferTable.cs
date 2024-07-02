namespace OrderManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOfferTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OfferTable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        offerId = c.String(),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OfferTable");
        }
    }
}
