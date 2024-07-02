namespace OrderManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOfferBillingEntriesTable2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfferBillingEntriesTable", "Name", c => c.String());
            AddColumn("dbo.OfferBillingEntriesTable", "description", c => c.String());
            AddColumn("dbo.OfferBillingEntriesTable", "quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.OfferBillingEntriesTable", "price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OfferBillingEntriesTable", "price", c => c.String());
            DropColumn("dbo.OfferBillingEntriesTable", "quantity");
            DropColumn("dbo.OfferBillingEntriesTable", "description");
            DropColumn("dbo.OfferBillingEntriesTable", "Name");
        }
    }
}
