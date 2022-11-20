using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZGenCodeGenerator.TemplateHandling
{
    public interface ITemplateHandler
    {
        Task CreateTemplate(IList<string> args);
        Task Generate(string templateName, IList<string> args);
    }
}
