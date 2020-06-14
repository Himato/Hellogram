namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class SeedingDatabase : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM dbo.Categories;");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Animals\', \'animals.jpg\', \'Animals are multi-cellular eukaryotic organisms that form the biological kingdom Animalia.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Architecture\', \'architecture.jpg\', \'Architecture is the art and science of designing buildings and other physical structures.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Arts & Culture\', \'arts.jpg\', \'Art & Culture are the diverse range of human activities in creating visual, auditory or performing artifacts (artworks), expressing the author s imaginative, conceptual ideas, or technical skill.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Business & Work\', \'business.jpg\', \'Business is the activity of making one s living or making money by producing or buying and selling products (such as goods and services).\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Engineering\', \'engineering.jpg\', \'Engineering is the use of scientific principles to design and build machines, structures, and other items, including bridges, tunnels, roads, vehicles, and buildings.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Fashion\', \'fashion.jpg\', \'Fashion is a popular aesthetic expression in a certain time and context, especially in clothing, footwear, lifestyle, accessories, makeup, hairstyle and body proportions.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Food & Drink\', \'food.jpg\', \'Food is any substance consumed to provide nutritional support for an organism.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Health\', \'health.jpg\', \'Health is a state of physical, mental and social well-being in which disease and infirmity are absent.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'History\', \'history.jpg\', \'History is the past as it is described in written documents, and the study thereof.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Learning\', \'learning.jpg\', \'Learning is the process of acquiring new, or modifying existing, knowledge, behaviors, skills, values, or preferences.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Media\', \'media.jpg\', \'Media are the communication outlets or tools used to store and deliver information or data.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Motivational\', \'motivational.jpg\', \'Motivation is the experience of desire or aversion…You want something, or want to avoid or escape something.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Nature\', \'nature.jpg\', \'Nature, in the broadest sense, is the natural, physical, or material world or universe. Nature can refer to the phenomena of the physical world, and also to life in general.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'News\', \'news.jpg\', \'Current events that happens around the world.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'People\', \'people.jpg\', \'A people is a plurality of persons considered as a whole, as is the case with an ethnic group or nation.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Science\', \'science.jpg\', \'Science is a systematic enterprise that builds and organizes knowledge in the form of testable explanations and predictions about the universe.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Technology\', \'technology.jpg\', \'Technology is the sum of techniques, skills, methods, and processes used in the production of goods or services or in the accomplishment of objectives, such as scientific investigation.\');");
            Sql("INSERT INTO dbo.Categories (Name, Image, Description) VALUES (\'Travel\', \'travel.jpg\', \'Travel is the movement of people between distant geographical locations.\');");
        }

        public override void Down()
        {
        }
    }
}