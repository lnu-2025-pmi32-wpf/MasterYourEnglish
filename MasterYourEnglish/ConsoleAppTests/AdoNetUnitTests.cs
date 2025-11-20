using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Npgsql;
using MasterYourEnglishApp;
using System.Linq;

public class AdoNetUnitTests
{
    private readonly Mock<ISqlExecutor> sqlExecutorMock;
    private readonly DatabaseSeeder seeder;

    public AdoNetUnitTests()
    {
        sqlExecutorMock = new Mock<ISqlExecutor>();
        seeder = new DatabaseSeeder(sqlExecutorMock.Object);


        sqlExecutorMock.Setup(x => x.ExecuteNonQueryAsync(
            It.IsAny<string>(),
            It.IsAny<IEnumerable<NpgsqlParameter>>()
        )).ReturnsAsync(1);
    }



    [Fact]
    public async Task ClearTables_ShouldExecuteNonQueryWithTruncateCommand()
    {
        sqlExecutorMock.Setup(x => x.ExecuteNonQueryAsync(
            It.Is<string>(sql => sql.Contains("TRUNCATE TABLE")),
            It.IsAny<IEnumerable<NpgsqlParameter>>()
        )).ReturnsAsync(0);

        await seeder.ClearTables();

        sqlExecutorMock.Verify(
            x => x.ExecuteNonQueryAsync(
                It.Is<string>(sql => sql.Contains("TRUNCATE TABLE")),
                It.IsAny<IEnumerable<NpgsqlParameter>>()
            ),
            Times.Once
        );
    }


    [Fact]
    public async Task PopulateDatabase_ShouldCallInsertUser30Times()
    {
        const int EXPECTED_COUNT = 30;
        int idCounter = 1;




        sqlExecutorMock.Setup(x => x.ExecuteScalarAsync(
            It.IsAny<string>(),
            It.IsAny<IEnumerable<NpgsqlParameter>>()
        ))

        .ReturnsAsync(() => { return idCounter++; });


        await seeder.PopulateDatabaseWithTestData();


        sqlExecutorMock.Verify(
            x => x.ExecuteScalarAsync(
                It.Is<string>(sql => sql.Contains("INSERT INTO users")),
                It.IsAny<IEnumerable<NpgsqlParameter>>()
            ),
            Times.Exactly(EXPECTED_COUNT)
        );
    }


    [Fact]
    public async Task PopulateDatabase_ShouldCallInsertTopic7Times()
    {
        const int EXPECTED_COUNT = 7;
        int idCounter = 1;


        sqlExecutorMock.Setup(x => x.ExecuteScalarAsync(
            It.IsAny<string>(),
            It.IsAny<IEnumerable<NpgsqlParameter>>()
        )).ReturnsAsync(() => { return idCounter++; });


        await seeder.PopulateDatabaseWithTestData();


        sqlExecutorMock.Verify(
            x => x.ExecuteScalarAsync(
                It.Is<string>(sql => sql.Contains("INSERT INTO topics")),
                It.IsAny<IEnumerable<NpgsqlParameter>>()
            ),
            Times.Exactly(EXPECTED_COUNT)
        );
    }


    [Fact]
    public async Task PopulateDatabase_ShouldCallInsertFlashcard100Times()
    {
        const int EXPECTED_COUNT = 100;
        int idCounter = 1;


        sqlExecutorMock.Setup(x => x.ExecuteScalarAsync(
            It.IsAny<string>(),
            It.IsAny<IEnumerable<NpgsqlParameter>>()
        )).ReturnsAsync(() => { return idCounter++; });

        // ACT
        await seeder.PopulateDatabaseWithTestData();


        sqlExecutorMock.Verify(
            x => x.ExecuteScalarAsync(
                It.Is<string>(sql => sql.Contains("INSERT INTO flashcards") && !sql.Contains("bundle")),
                It.IsAny<IEnumerable<NpgsqlParameter>>()
            ),
            Times.Exactly(EXPECTED_COUNT)
        );
    }
}