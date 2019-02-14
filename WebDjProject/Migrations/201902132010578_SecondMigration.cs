namespace WebDjProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Playlists",
                c => new
                    {
                        playlistId = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        playlistName = c.String(),
                        songCount = c.Int(nullable: false),
                        spotifyPlaylistID = c.String(),
                    })
                .PrimaryKey(t => t.playlistId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Playlists", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Playlists", new[] { "ApplicationUserId" });
            DropTable("dbo.Playlists");
        }
    }
}
