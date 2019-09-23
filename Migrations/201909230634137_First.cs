namespace FilmLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Actors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name);
            
            CreateTable(
                "dbo.Films",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Producer = c.String(maxLength: 100),
                        GanreId = c.Int(),
                        Description = c.String(),
                        Budget = c.Double(nullable: false),
                        Score = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ganres", t => t.GanreId)
                .Index(t => t.Producer)
                .Index(t => t.GanreId);
            
            CreateTable(
                "dbo.Ganres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name);
            
            CreateTable(
                "dbo.FilmActors",
                c => new
                    {
                        Film_Id = c.Int(nullable: false),
                        Actor_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Film_Id, t.Actor_Id })
                .ForeignKey("dbo.Films", t => t.Film_Id, cascadeDelete: true)
                .ForeignKey("dbo.Actors", t => t.Actor_Id, cascadeDelete: true)
                .Index(t => t.Film_Id)
                .Index(t => t.Actor_Id);
            
            CreateStoredProcedure(
                "dbo.Actor_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 100),
                    },
                body:
                    @"INSERT [dbo].[Actors]([Name])
                      VALUES (@Name)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Actors]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Actors] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Actor_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(maxLength: 100),
                    },
                body:
                    @"UPDATE [dbo].[Actors]
                      SET [Name] = @Name
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Actor_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Actors]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Film_Insert",
                p => new
                    {
                        Name = p.String(),
                        Producer = p.String(maxLength: 100),
                        GanreId = p.Int(),
                        Description = p.String(),
                        Budget = p.Double(),
                        Score = p.Double(),
                    },
                body:
                    @"INSERT [dbo].[Films]([Name], [Producer], [GanreId], [Description], [Budget], [Score])
                      VALUES (@Name, @Producer, @GanreId, @Description, @Budget, @Score)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Films]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Films] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Film_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(),
                        Producer = p.String(maxLength: 100),
                        GanreId = p.Int(),
                        Description = p.String(),
                        Budget = p.Double(),
                        Score = p.Double(),
                    },
                body:
                    @"UPDATE [dbo].[Films]
                      SET [Name] = @Name, [Producer] = @Producer, [GanreId] = @GanreId, [Description] = @Description, [Budget] = @Budget, [Score] = @Score
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Film_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Films]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Ganre_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 100),
                    },
                body:
                    @"INSERT [dbo].[Ganres]([Name])
                      VALUES (@Name)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Ganres]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Ganres] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Ganre_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(maxLength: 100),
                    },
                body:
                    @"UPDATE [dbo].[Ganres]
                      SET [Name] = @Name
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Ganre_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Ganres]
                      WHERE ([Id] = @Id)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Ganre_Delete");
            DropStoredProcedure("dbo.Ganre_Update");
            DropStoredProcedure("dbo.Ganre_Insert");
            DropStoredProcedure("dbo.Film_Delete");
            DropStoredProcedure("dbo.Film_Update");
            DropStoredProcedure("dbo.Film_Insert");
            DropStoredProcedure("dbo.Actor_Delete");
            DropStoredProcedure("dbo.Actor_Update");
            DropStoredProcedure("dbo.Actor_Insert");
            DropForeignKey("dbo.Films", "GanreId", "dbo.Ganres");
            DropForeignKey("dbo.FilmActors", "Actor_Id", "dbo.Actors");
            DropForeignKey("dbo.FilmActors", "Film_Id", "dbo.Films");
            DropIndex("dbo.FilmActors", new[] { "Actor_Id" });
            DropIndex("dbo.FilmActors", new[] { "Film_Id" });
            DropIndex("dbo.Ganres", new[] { "Name" });
            DropIndex("dbo.Films", new[] { "GanreId" });
            DropIndex("dbo.Films", new[] { "Producer" });
            DropIndex("dbo.Actors", new[] { "Name" });
            DropTable("dbo.FilmActors");
            DropTable("dbo.Ganres");
            DropTable("dbo.Films");
            DropTable("dbo.Actors");
        }
    }
}
