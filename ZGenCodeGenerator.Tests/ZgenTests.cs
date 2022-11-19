using Moq;
using ZGenCodeGenerator.FileHandling;
using ZGenCodeGenerator.generators;

namespace ZGenCodeGenerator.Tests
{
    public class Zgentests
    {
        private readonly Mock<IFileHandler> _fileMock;
        private readonly ZGenerator _generator;


        public Zgentests()
        {
            _fileMock = new Mock<IFileHandler>();
            _generator = new ZGenerator(_fileMock.Object);
        }
        [Fact]
        public async Task CanWriteDirFromTempl()
        {
            _fileMock.Setup(x => x.GetCurrDir())
                .Returns(Task.FromResult("c:\\temp"));
            _fileMock.Setup(x => x.GetTemplatePath("test"))
                .Returns(Task.FromResult("c:\\temp\\.zgentemplates"));
            _fileMock.Setup(x => x.GetDirectories("c:\\temp\\.zgentemplates"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z" });
            _fileMock.Setup(x => x.GetDirectories("c:\\temp\\.zgentemplates\\z"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z\\test" });
            _fileMock.Setup(x => x.GetDirectories("c:\\temp\\.zgentemplates\\z\\test"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z\\test\\az1" });

            _fileMock.Setup(x => x.DirectoryExist(It.IsAny<string>()))
                .Returns(true);

            bool addedDir = false;
            _fileMock.Setup(x => x.AddDirectory("c:\\temp\\asdf\\afoo"))
                .Callback(() =>
            {
                addedDir = true;
            });
            Dictionary<string, string> varnames = new();
            varnames.Add("z1", "foo");
            await _generator.Generate("c:\\temp\\.zgentemplates\\z\\test",
                "c:\\temp\\asdf", varnames, new List<string>());
            Assert.True(addedDir);
        }

        [Fact]
        public async Task CanWriteFile()
        {
            _fileMock.Setup(x => x.GetCurrDir())
                .Returns(Task.FromResult("c:\\temp"));
            _fileMock.Setup(x => x.GetTemplatePath("test"))
                .Returns(Task.FromResult("c:\\temp\\.zgentemplates"));
            _fileMock.Setup(x => x.GetDirectories("c:\\temp\\.zgentemplates"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z" });
            _fileMock.Setup(x => x.GetDirectories("c:\\temp\\.zgentemplates\\z"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z\\test" });
            _fileMock.Setup(x => x.GetDirectories("c:\\temp\\.zgentemplates\\z\\test"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z\\test\\az1" });
            _fileMock.Setup(x => x.GetFiles("c:\\temp\\.zgentemplates\\z\\test\\az1"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z\\test\\bz2.txt" });
            _fileMock.Setup(x => x.ReadAllTextAsync("c:\\temp\\.zgentemplates\\z\\test\\bz2.txt"))
                .Returns(Task.FromResult("asdf{{z1}}"));
            _fileMock.Setup(x => x.DirectoryExist(It.IsAny<string>()))
                .Returns(true);

            Dictionary<string, string> varnames = new();
            varnames.Add("z1", "foo");
            varnames.Add("z2", "bar");
            await _generator.Generate("c:\\temp\\.zgentemplates\\z\\test",
                "c:\\temp\\asdf", varnames, new List<string>());
            _fileMock.Verify(x => x.WriteAllTextAsync("c:\\temp\\asdf\\bbar.txt", "asdffoo"));
        }
    }
}