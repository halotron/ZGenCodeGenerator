using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using ZGenCodeGenerator.Exceptions;
using ZGenCodeGenerator.FileHandling;
using ZGenCodeGenerator.generators;
using ZGenCodeGenerator.TemplateHandling;

namespace ZGenCodeGenerator
{
    internal class Program
    {
        static IFileHandler _fileHandler;
        static ITemplateHandler _templateHandler;
        static async Task Main(string[] args)
        {
            _fileHandler = new FileHandler();
            _templateHandler = new TemplateHandler(_fileHandler,
                new Lazy<IGeneratorFactory>(() => new GeneratorFactory(_fileHandler)));
            var templateNames = _fileHandler.GetTemplateNames();
            if (args.Length == 0)
            {
                await ShowHelp(templateNames);
                return;
            }
            var firstAarg = args[0].ToLower();
            if (firstAarg == "-h" || firstAarg == "--help")
            {
                await ShowHelp(templateNames);
                return;
            }
            else if (firstAarg == "gen")
            {
                await _templateHandler.CreateTemplate(args.Skip(1).ToList());
            }
            else
            {
                var names = (await templateNames).Select(x => x.ToLower()).ToList();
                if (names.Contains(firstAarg))
                {
                    try
                    {
                        await _templateHandler.Generate(firstAarg, args.Skip(1).ToList());

                    }
                    catch (NumberOfVariableParametersNotMatchingException e)
                    {
                        Console.WriteLine($"The template {e.TemplateName} requires {e.Variables?.Count() ?? 0} parameters, but you only provided {e.ProvidedParameters?.Count() ?? 0} parameters");
                    }

                }
                else
                {
                    await ShowHelp(null, names);
                }

            }
            return;
        }






        private static async Task ShowHelp(Task<IList<string>> templateNames, IList<string> resolvedNames = null)
        {

            Console.WriteLine("ZGen Code Generator");
            Console.WriteLine(@"
Options:
    gen - Generates a new template using a wizard
    <template name> - executes that template

If template name is the first argument, the following command line arguments are passed to the template:
    <template name> <arg1> <arg2> <arg3> ...
 
Available templates:
");
            IList<string> names;
            if (resolvedNames != null)
                names = resolvedNames;
            else
                names = await templateNames;
            if (names != null && names.Count > 0)
            {
                names.ToList().ForEach(t => Console.WriteLine(t));
            }
            else
            {
                Console.WriteLine("No templates found");
                Console.WriteLine("Create one with the 'gen' command");
            }
        }


    }
}
