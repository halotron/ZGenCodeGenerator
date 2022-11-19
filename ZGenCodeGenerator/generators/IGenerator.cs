using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ZGenCodeGenerator.generators
{
    public interface IGenerator
    {
        Task<IEnumerable<string>> GetTemplateVariables(string templateDir);
        Task Generate(string templateDir, string targetDir, Dictionary<string, string> varnames, IList<string> args);
        Task CreateTemplate(string templateDir);
    }
}
