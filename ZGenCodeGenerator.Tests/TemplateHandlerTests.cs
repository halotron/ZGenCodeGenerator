using Moq;
using ZGenCodeGenerator.Exceptions;
using ZGenCodeGenerator.FileHandling;
using ZGenCodeGenerator.generators;
using ZGenCodeGenerator.TemplateHandling;

namespace ZGenCodeGenerator.Tests
{
    public class TemplateHandlerTests
    {
        private readonly Mock<IFileHandler> _fileMock;
        private readonly Mock<IGeneratorFactory> _generatorFactory;
        private readonly Mock<IGenerator> _generator;
        private readonly ITemplateHandler _templateHandler;

        public TemplateHandlerTests()
        {
            _fileMock = new Mock<IFileHandler>();
            _generatorFactory = new Mock<IGeneratorFactory>();
            _generator = new Mock<IGenerator>();
            _templateHandler = new TemplateHandler(_fileMock.Object,
                new Lazy<IGeneratorFactory>(() => _generatorFactory.Object));

            _generatorFactory.Setup(x => x.GetGenerator(It.IsAny<string>())).Returns(_generator.Object);

        }

        [Fact]
        public async Task CanNotCreateExistingTemplateName()
        {
            List<string> args = CreateTesttemplate();
            await Assert.ThrowsAsync<TemplateAlreadyExistException>(async () => await _templateHandler.CreateTemplate(args));
        }

        private List<string> CreateTesttemplate()
        {
            _fileMock.Setup(x => x.GetCurrDir())
                .Returns(Task.FromResult("c:\\temp"));
            _fileMock.Setup(x => x.GetTemplatePath("test"))
                .Returns(Task.FromResult("c:\\temp\\.zgentemplates\\z\\test"));

            var args = new List<string> { "z", "test" };
            return args;
        }

        [Fact]
        public async Task CanCreateTemplateName()
        {
            _fileMock.Setup(x => x.GetCurrDir())
                .Returns(Task.FromResult("c:\\temp"));
            _fileMock.Setup(x => x.GetTemplatePath("test"))
                .Returns(Task.FromResult(null as string));
            _fileMock.Setup(x => x.GetExistingTemplateDir())
                .Returns(Task.FromResult("c:\\temp\\.zgentemplates"));

            var args = new List<string> { "z", "test", "2" };
            await _templateHandler.CreateTemplate(args);
            _generator.Verify(x => x.CreateTemplate("c:\\temp\\.zgentemplates\\z\\test"), Times.Once);
        }


        [Fact]
        public async Task CanNotGenerateNotExistingTemplate()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _templateHandler.Generate("not_existing", new List<string>()));
        }

        [Fact]
        public async Task GenerateTooFewParameters()
        {
            _generator.Setup(x => x.GetTemplateVariables(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    ((new List<string> { "var1", "var2", "var3" })) as IList<string>)
                    );



            List<string> args = CreateTesttemplate();
            await Assert.ThrowsAsync<NumberOfVariableParametersNotMatchingException>(
                () =>
                _templateHandler.Generate("test", args)
                );
        }
        [Fact]
        public async Task GenerateTooManyParameters()
        {
            _generator.Setup(x => x.GetTemplateVariables(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    ((new List<string> { "var1", "var2" })) as IList<string>)
                    );  

            List<string> args = CreateTesttemplate();
            args.Add("asdf");
            args.Add("asdf");
            args.Add("asdf");
            args.Add("asdf");
            args.Add("asdf");
            await Assert.ThrowsAsync<NumberOfVariableParametersNotMatchingException>(
                () =>
                _templateHandler.Generate("test", args)
                );
        }
    }
}