using System;
using System.Collections.Generic;
using System.Text;

namespace ZGenCodeGenerator.generators
{
    public interface IGeneratorFactory
    {
        IGenerator GetGenerator(string typeOfTemplate);
    }
}
