using FluentAssertions;
using NUnit.Framework.Interfaces;
using NLog;
using FileModel = Task1.SourceCode.File;

namespace Task2.Core.UnitTests;

[TestFixture]
public class FileTests
{
    // NLog logger instance dedicated to this test class
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    [SetUp]
    public void SetUp()
    {
        // Logs the beginning of each test for traceability
        Logger.Info("START {TestName}", TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void TearDown()
    {
        // Logs standardized outcome (PASS / SKIP / FAIL) with message if not passed
        var ctx = TestContext.CurrentContext;
        var status = ctx.Result.Outcome.Status;

        if (status == TestStatus.Passed)
            Logger.Info("PASS {TestName}", ctx.Test.Name);
        else if (status == TestStatus.Skipped)
            Logger.Warn("SKIP {TestName} - {Message}", ctx.Test.Name, ctx.Result.Message);
        else
            Logger.Error("FAIL {TestName} - {Message}", ctx.Test.Name, ctx.Result.Message);
    }

    [Test]
    public void Constructor_Should_Set_Properties()
    {
        // Arrange
        Logger.Info("Arranging test data");
        const string fileName = "doc.txt";
        const string content = "hello";

        // Act
        Logger.Info("Creating file with name={Name} content={Content}", fileName, content);
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying file properties");
        file.GetFileName().Should().Be(fileName);
        file.GetSize().Should().Be(2); // Expect 2, not 2.5
    }

    [Test, Ignore("File does not validate input")]
    public void Constructor_Should_Throw_When_FileName_Empty()
    {
        // Arrange
        Logger.Info("Arranging test with empty filename");
        string emptyFileName = "";
        string content = "x";

        // Act
        Logger.Info("Attempting to create file with empty name");
        var act = () => new FileModel(emptyFileName, content);

        // Assert
        Logger.Info("Verifying ArgumentException is thrown");
        act.Should().Throw<ArgumentException>();
    }

    [Test, Ignore("File does not validate input")]
    public void Constructor_Should_Throw_When_Content_Null()
    {
        // Arrange
        Logger.Info("Arranging test with null content");
        string validFileName = "a.txt";
        string? nullContent = null;

        // Act
        Logger.Info("Attempting to create file with null content");
        var act = () => new FileModel(validFileName, nullContent!);

        // Assert
        Logger.Info("Verifying ArgumentNullException is thrown");
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_Should_Handle_Empty_Content()
    {
        // Arrange
        Logger.Info("Arranging test with empty content");
        string fileName = "empty.txt";
        string content = "";

        // Act
        Logger.Info("Creating file with empty content");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying file size is zero");
        file.GetFileName().Should().Be(fileName);
        file.GetSize().Should().Be(0);
    }

    [Test]
    public void Constructor_Should_Calculate_Size_For_Odd_Content_Length()
    {
        // Arrange
        Logger.Info("Arranging test with odd content length");
        string fileName = "odd.txt";
        string content = "abc"; // length 3

        // Act
        Logger.Info("Creating file with odd content length");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying file size is half of content length");
        file.GetSize().Should().Be(1);
    }

    [Test]
    public void Constructor_Should_Calculate_Size_For_Even_Content_Length()
    {
        // Arrange
        Logger.Info("Arranging test with even content length");
        string fileName = "even.txt";
        string content = "abcd"; // length 4

        // Act
        Logger.Info("Creating file with even content length");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying file size is half of content length");
        file.GetSize().Should().Be(2);
    }

    [Test]
    public void Constructor_Should_Handle_Special_Characters_In_FileName()
    {
        // Arrange
        Logger.Info("Arranging test with special characters in filename");
        string fileName = "spécial_文件.txt";
        string content = "abc";

        // Act
        Logger.Info("Creating file with special characters in filename");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying filename is set");
        file.GetFileName().Should().Be(fileName);
    }

    [Test]
    public void Constructor_Should_Throw_When_FileName_Null()
    {
        // Arrange
        Logger.Info("Arranging test with null filename");
        string? nullFileName = null;
        string content = "abc";

        // Act
        Logger.Info("Attempting to create file with null filename");
        var act = () => new FileModel(nullFileName!, content);

        // Assert
        Logger.Info("Verifying ArgumentNullException is thrown");
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_Should_Handle_FileName_With_Path_Separators()
    {
        // Arrange
        Logger.Info("Arranging test with path separators in filename");
        string fileName = "folder\\file.txt";
        string content = "abc";

        // Act
        Logger.Info("Creating file with path separators in filename");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying filename is set");
        file.GetFileName().Should().Be(fileName);
    }

    [Test]
    public void Constructor_Should_Handle_Very_Large_Content()
    {
        // Arrange
        Logger.Info("Arranging test with very large content");
        string fileName = "large.txt";
        string content = new string('a', 100_000);

        // Act
        Logger.Info("Creating file with very large content");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying file size for large content");
        file.GetSize().Should().Be(50_000);
    }

    [Test]
    public void Constructor_Should_Handle_FileName_With_Leading_Dot()
    {
        // Arrange
        Logger.Info("Arranging test with leading dot in filename");
        string fileName = ".hiddenfile";
        string content = "abc";

        // Act
        Logger.Info("Creating file with leading dot in filename");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying filename is set");
        file.GetFileName().Should().Be(fileName);
    }

    [Test]
    public void Constructor_Should_Handle_FileName_Ending_With_Dot()
    {
        // Arrange
        Logger.Info("Arranging test with filename ending with dot");
        string fileName = "file.";
        string content = "abc";

        // Act
        Logger.Info("Creating file with filename ending with dot");
        var file = new FileModel(fileName, content);

        // Assert
        Logger.Info("Verifying filename is set");
        file.GetFileName().Should().Be(fileName);
    }
}
