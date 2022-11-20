using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZGenCodeGenerator.Models;

namespace ZGenCodeGenerator.TemplateHandling
{
    public interface ITemplateHandler
    {
        Task<IList<SourceGeneratorInfo>> GetSourceGenerators();
        Task<string> CreateTemplate(IList<string> args);
        Task Generate(string templateName, IList<string> args);
    }
}
