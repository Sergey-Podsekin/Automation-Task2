using FluentAssertions;
using NUnit.Framework.Interfaces;
using NLog;
using Task1.SourceCode;
using Task1.SourceCode.exception;
using FileModel = Task1.SourceCode.File;

namespace Task2.Core.UnitTests;

[TestFixture]
public class FileStorageTests
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    [SetUp]
    public void SetUp()
    {
        Logger.Info("START {TestName}", TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void TearDown()
    {
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
    public void Write_Should_Save_File_When_Unique_And_SpaceAvailable()
    {
        // Arrange
        Logger.Info("Arranging storage and file");
        var storage = new FileStorage(50);
        var file = new FileModel("a.txt", "abcd");

        // Act
        Logger.Info("Writing file to storage");
        var result = storage.Write(file);

        // Assert
        Logger.Info("Verifying file was written successfully");
        result.Should().BeTrue("file should fit and be unique");
        storage.GetFiles().Should().Contain(file);
    }

    [Test]
    public void Write_Should_Throw_When_Duplicate()
    {
        // Arrange
        Logger.Info("Arranging storage with existing file");
        var storage = new FileStorage();
        var originalFile = new FileModel("dup.txt", "a");
        storage.Write(originalFile);

        // Act
        Logger.Info("Attempting to write duplicate file");
        var act = () => storage.Write(new FileModel("dup.txt", "b"));

        // Assert
        Logger.Info("Verifying duplicate file throws exception");
        act.Should().Throw<Exception>()
           .Where(e => e.GetType().Name == "FileNameAlreadyExistsException");
    }

    [Test]
    public void Write_Should_Return_False_When_NotEnoughSpace()
    {
        // Arrange
        Logger.Info("Arranging small storage and large file");
        var storage = new FileStorage(1);
        var bigFile = new FileModel("big.txt", new string('a', 100));

        // Act
        Logger.Info("Attempting to write file larger than storage");
        var result = storage.Write(bigFile);

        // Assert
        Logger.Info("Verifying write operation failed");
        result.Should().BeFalse();
    }

    [Test]
    public void Delete_Should_Remove_File()
    {
        // Arrange
        Logger.Info("Arranging storage with file to delete");
        var storage = new FileStorage();
        var file = new FileModel("d.txt", "x");
        storage.Write(file);

        // Act
        Logger.Info("Deleting file");
        var deleteResult = storage.Delete("d.txt");

        // Assert
        Logger.Info("Verifying file was deleted");
        deleteResult.Should().BeTrue("file should exist and be removable");
        storage.IsExists("d.txt").Should().BeFalse();
    }

    [Test]
    public void GetFile_Should_Return_File_If_Exists()
    {
        // Arrange
        Logger.Info("Arranging storage with test file");
        var storage = new FileStorage();
        var file = new FileModel("ok.txt", "y");
        storage.Write(file);

        // Act
        Logger.Info("Retrieving file");
        var found = storage.GetFile("ok.txt");

        // Assert
        Logger.Info("Verifying file was found");
        found.Should().NotBeNull();
        found!.GetFileName().Should().Be("ok.txt");
    }

    [Test]
    public void GetFile_Should_Return_Null_If_NotFound()
    {
        // Arrange
        Logger.Info("Arranging empty storage");
        var storage = new FileStorage();

        // Act
        Logger.Info("Attempting to get non-existent file");
        var found = storage.GetFile("missing.txt");

        // Assert
        Logger.Info("Verifying null result");
        found.Should().BeNull();
    }

    [Test]
    public void IsExists_Should_Return_True_For_Existing_File()
    {
        // Arrange
        Logger.Info("Arranging storage with file");
        var storage = new FileStorage();
        var file = new FileModel("exists.txt", "abc");
        storage.Write(file);

        // Act
        Logger.Info("Checking existence of file");
        var exists = storage.IsExists("exists.txt");

        // Assert
        Logger.Info("Verifying file exists");
        exists.Should().BeTrue();
    }

    [Test]
    public void IsExists_Should_Return_False_For_NonExisting_File()
    {
        // Arrange
        Logger.Info("Arranging empty storage");
        var storage = new FileStorage();

        // Act
        Logger.Info("Checking existence of non-existent file");
        var exists = storage.IsExists("nope.txt");

        // Assert
        Logger.Info("Verifying file does not exist");
        exists.Should().BeFalse();
    }

    [Test]
    public void Delete_Should_Return_False_If_File_Not_Found()
    {
        // Arrange
        Logger.Info("Arranging storage without target file");
        var storage = new FileStorage();

        // Act
        Logger.Info("Attempting to delete non-existent file");
        var result = storage.Delete("ghost.txt");

        // Assert
        Logger.Info("Verifying delete returns false");
        result.Should().BeFalse();
    }

    [Test]
    public void Write_Should_Handle_File_With_Special_Characters_In_Name()
    {
        // Arrange
        Logger.Info("Arranging storage and file with special characters");
        var storage = new FileStorage();
        var file = new FileModel("spécial_文件.txt", "abc");

        // Act
        Logger.Info("Writing file with special characters");
        var result = storage.Write(file);

        // Assert
        Logger.Info("Verifying file was written");
        result.Should().BeTrue();
        storage.IsExists("spécial_文件.txt").Should().BeTrue();
    }

    [Test]
    public void Write_Should_Handle_File_With_Zero_Size()
    {
        // Arrange
        Logger.Info("Arranging storage and empty file");
        var storage = new FileStorage();
        var file = new FileModel("empty.txt", "");

        // Act
        Logger.Info("Writing empty file");
        var result = storage.Write(file);

        // Assert
        Logger.Info("Verifying empty file was written");
        result.Should().BeTrue();
        storage.IsExists("empty.txt").Should().BeTrue();
    }

    [Test]
    public void GetFiles_Should_Return_All_Added_Files()
    {
        // Arrange
        Logger.Info("Arranging storage with multiple files");
        var storage = new FileStorage();
        var file1 = new FileModel("f1.txt", "a");
        var file2 = new FileModel("f2.txt", "bb");
        storage.Write(file1);
        storage.Write(file2);

        // Act
        Logger.Info("Getting all files");
        var files = storage.GetFiles();

        // Assert
        Logger.Info("Verifying all files are returned");
        files.Should().Contain(file1);
        files.Should().Contain(file2);
        files.Count.Should().Be(2);
    }

    [Test]
    public void IsExists_Should_Return_True_For_Partial_FileName()
    {
        // Arrange
        Logger.Info("Arranging storage with file for partial match test");
        var storage = new FileStorage();
        var file = new FileModel("file.txt", "abc");
        storage.Write(file);

        // Act
        Logger.Info("Checking existence of file by partial name");
        var exists = storage.IsExists("ile.tx");

        // Assert
        Logger.Info("Verifying file exists for partial name check");
        exists.Should().BeTrue();
    }
}
