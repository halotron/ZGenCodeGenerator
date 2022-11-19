using System;
using System.Collections.Generic;
using System.Text;
using ZGenCodeGenerator.FileHandling;

namespace ZGenCodeGenerator.generators
{
    public class GeneratorFactory : IGeneratorFactory
    {
        private readonly IFileHandler _fileHandler;
        public GeneratorFactory(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public IGenerator GetGenerator(string typeOfTemplate)
        {
            IGenerator generator = null;
            switch (typeOfTemplate)
            {
                case "z":
                    generator = new ZGenerator(_fileHandler);
                    break;
                default:
                    break;
            }
            return generator;
        }
    }
}
