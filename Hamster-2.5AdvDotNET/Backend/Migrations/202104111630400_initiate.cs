namespace Backend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initiate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Aktivitets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hamsters",
                c => new
                    {
                        hamsterID = c.Int(nullable: false, identity: true),
                        Namn = c.String(),
                        Ålder = c.Int(nullable: false),
                        Kön = c.String(),
                        ÄgarNamn = c.String(),
                        IdentityNum = c.String(),
                        CheckIn = c.DateTime(nullable: false),
                        MotionsNivå = c.Int(nullable: false),
                        Aktivitet_Id = c.Int(),
                        Bur_Id = c.Int(),
                        HamsterKö_Id = c.Int(),
                        Hemfärd_Id = c.Int(),
                        ResterandeHamster_Id = c.Int(),
                    })
                .PrimaryKey(t => t.hamsterID)
                .ForeignKey("dbo.Aktivitets", t => t.Aktivitet_Id)
                .ForeignKey("dbo.Burs", t => t.Bur_Id)
                .ForeignKey("dbo.HamsterKö", t => t.HamsterKö_Id)
                .ForeignKey("dbo.Hemfärd", t => t.Hemfärd_Id)
                .ForeignKey("dbo.ResterandeHamsters", t => t.ResterandeHamster_Id)
                .Index(t => t.Aktivitet_Id)
                .Index(t => t.Bur_Id)
                .Index(t => t.HamsterKö_Id)
                .Index(t => t.Hemfärd_Id)
                .Index(t => t.ResterandeHamster_Id);
            
            CreateTable(
                "dbo.Burs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HamsterKö",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hemfärd",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResterandeHamsters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hamsters", "ResterandeHamster_Id", "dbo.ResterandeHamsters");
            DropForeignKey("dbo.Hamsters", "Hemfärd_Id", "dbo.Hemfärd");
            DropForeignKey("dbo.Hamsters", "HamsterKö_Id", "dbo.HamsterKö");
            DropForeignKey("dbo.Hamsters", "Bur_Id", "dbo.Burs");
            DropForeignKey("dbo.Hamsters", "Aktivitet_Id", "dbo.Aktivitets");
            DropIndex("dbo.Hamsters", new[] { "ResterandeHamster_Id" });
            DropIndex("dbo.Hamsters", new[] { "Hemfärd_Id" });
            DropIndex("dbo.Hamsters", new[] { "HamsterKö_Id" });
            DropIndex("dbo.Hamsters", new[] { "Bur_Id" });
            DropIndex("dbo.Hamsters", new[] { "Aktivitet_Id" });
            DropTable("dbo.ResterandeHamsters");
            DropTable("dbo.Hemfärd");
            DropTable("dbo.HamsterKö");
            DropTable("dbo.Burs");
            DropTable("dbo.Hamsters");
            DropTable("dbo.Aktivitets");
        }
    }
}
