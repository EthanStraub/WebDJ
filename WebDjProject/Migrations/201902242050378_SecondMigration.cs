namespace WebDjProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFollows",
                c => new
                    {
                        UserFollowPairID = c.Int(nullable: false, identity: true),
                        followedID = c.String(nullable: false),
                        followedUserName = c.String(nullable: false),
                        followerID = c.String(nullable: false),
                        followerUserName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserFollowPairID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserFollows");
        }
    }
}
