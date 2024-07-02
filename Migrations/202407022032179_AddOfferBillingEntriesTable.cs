namespace OrderManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOfferBillingEntriesTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.BillingEntriesTable", newName: "OrdersBillingEntriesTable");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.OrdersBillingEntriesTable", newName: "BillingEntriesTable");
        }
    }
}
