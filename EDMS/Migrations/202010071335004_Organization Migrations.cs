namespace EDMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganizationMigrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        categoryId = c.Int(nullable: false, identity: true),
                        categoryname = c.String(),
                        divisionId = c.Int(nullable: false),
                        departmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.categoryId)
                .ForeignKey("dbo.Departments", t => t.departmentId, cascadeDelete: false)
                .ForeignKey("dbo.Divisions", t => t.divisionId, cascadeDelete: false)
                .Index(t => t.divisionId)
                .Index(t => t.departmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        departmentId = c.Int(nullable: false, identity: true),
                        departmentName = c.String(),
                    })
                .PrimaryKey(t => t.departmentId);
            
            CreateTable(
                "dbo.Divisions",
                c => new
                    {
                        divisionId = c.Int(nullable: false, identity: true),
                        divisionName = c.String(),
                        departmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.divisionId)
                .ForeignKey("dbo.Departments", t => t.departmentId, cascadeDelete: false)
                .Index(t => t.departmentId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        documentId = c.Int(nullable: false, identity: true),
                        documentName = c.String(),
                        documentPath = c.String(),
                        documentType = c.String(),
                        documentSize = c.Int(nullable: false),
                        departmentId = c.Int(nullable: false),
                        divisionid = c.Int(nullable: false),
                        categoryId = c.Int(nullable: false),
                        documentCreate = c.DateTime(nullable: false),
                        documentModify = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.documentId)
                .ForeignKey("dbo.Categories", t => t.categoryId, cascadeDelete: false)
                .ForeignKey("dbo.Departments", t => t.departmentId, cascadeDelete: false)
                .ForeignKey("dbo.Divisions", t => t.divisionid, cascadeDelete: false)
                .Index(t => t.departmentId)
                .Index(t => t.divisionid)
                .Index(t => t.categoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "divisionid", "dbo.Divisions");
            DropForeignKey("dbo.Documents", "departmentId", "dbo.Departments");
            DropForeignKey("dbo.Documents", "categoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "divisionId", "dbo.Divisions");
            DropForeignKey("dbo.Divisions", "departmentId", "dbo.Departments");
            DropForeignKey("dbo.Categories", "departmentId", "dbo.Departments");
            DropIndex("dbo.Documents", new[] { "categoryId" });
            DropIndex("dbo.Documents", new[] { "divisionid" });
            DropIndex("dbo.Documents", new[] { "departmentId" });
            DropIndex("dbo.Divisions", new[] { "departmentId" });
            DropIndex("dbo.Categories", new[] { "departmentId" });
            DropIndex("dbo.Categories", new[] { "divisionId" });
            DropTable("dbo.Documents");
            DropTable("dbo.Divisions");
            DropTable("dbo.Departments");
            DropTable("dbo.Categories");
        }
    }
}
