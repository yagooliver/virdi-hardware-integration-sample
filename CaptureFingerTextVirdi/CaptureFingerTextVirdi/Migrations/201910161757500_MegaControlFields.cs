namespace CaptureFingerTextVirdi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MegaControlFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Name", c => c.String());
            AddColumn("dbo.Users", "Document", c => c.String());
            AddColumn("dbo.Users", "PhoneNumber", c => c.String());
            AddColumn("dbo.Users", "Email", c => c.String());
            AddColumn("dbo.Users", "Nfp", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Bfp1", c => c.Binary());
            AddColumn("dbo.Users", "Bfp2", c => c.Binary());
            AddColumn("dbo.Users", "Bfp3", c => c.Binary());
            AddColumn("dbo.Users", "Bfp4", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Bfp4");
            DropColumn("dbo.Users", "Bfp3");
            DropColumn("dbo.Users", "Bfp2");
            DropColumn("dbo.Users", "Bfp1");
            DropColumn("dbo.Users", "Nfp");
            DropColumn("dbo.Users", "Email");
            DropColumn("dbo.Users", "PhoneNumber");
            DropColumn("dbo.Users", "Document");
            DropColumn("dbo.Users", "Name");
        }
    }
}
