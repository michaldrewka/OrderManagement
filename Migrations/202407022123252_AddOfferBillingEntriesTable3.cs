namespace OrderManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOfferBillingEntriesTable3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OfferBillingEntriesTable", "description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OfferBillingEntriesTable", "description", c => c.String());
        }
    }
}
