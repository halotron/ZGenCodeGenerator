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
                "c:\\temp\\asdf", varnames);
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
                "c:\\temp\\asdf", varnames);
            _fileMock.Verify(x => x.WriteAllTextAsync("c:\\temp\\asdf\\bbar.txt", "asdffoo"));
        }


        [Fact]
        public async Task VariablesReturnedInCorrectOrder()
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
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z\\test\\az7" });
            _fileMock.Setup(x => x.GetFiles("c:\\temp\\.zgentemplates\\z\\test\\az7"))
                .Returns(new List<string> { "c:\\temp\\.zgentemplates\\z\\test\\bz2.txt" });
            _fileMock.Setup(x => x.ReadAllTextAsync("c:\\temp\\.zgentemplates\\z\\test\\bz2.txt"))
                .Returns(Task.FromResult("asdf{{z1}} och luring {{z11}}"));
            _fileMock.Setup(x => x.DirectoryExist(It.IsAny<string>()))
                .Returns(true);

            var variables = (await _generator.GetTemplateVariables("test")).ToList();
            Assert.Equal(4, variables.Count());
            Assert.Equal("z1", variables[0]);
            Assert.Equal("z2", variables[1]);
            Assert.Equal("z7", variables[2]);
            Assert.Equal("z11", variables[3]);

        }

        [Fact]
        public async Task CanNotSupplyNullForMappingDictionary()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _generator.Generate("test", null, null));
        }

    }
}
