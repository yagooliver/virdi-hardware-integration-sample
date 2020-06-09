namespace CaptureFingerTextVirdi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserImported : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserImported",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 100),
                        UserId = c.String(nullable: false, maxLength: 100),
                        Rfid = c.String(maxLength: 100),
                        FingerText = c.Binary(),
                        Name = c.String(),
                        Document = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserImported");
        }
    }
}
