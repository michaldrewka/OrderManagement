namespace OrderManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOfferBillingEntriesTable1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OfferBillingEntriesTable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        offerId = c.String(),
                        price = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OfferBillingEntriesTable");
        }
    }
}
