using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ZGenCodeGenerator.Exceptions;
using ZGenCodeGenerator.FileHandling;
using ZGenCodeGenerator.generators;
using ZGenCodeGenerator.Models;
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
            var templateNames = _fileHandler.GetTemplateInfos();
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
                if (args.Length == 1)
                {
                    Console.WriteLine("You need to specify what type of source generator to create.");
                    Console.WriteLine("Currently, these are supported:");
                    var supportedCodeGenerators = await _templateHandler.GetSourceGenerators();
                    foreach (var item in supportedCodeGenerators)
                    {
                        Console.WriteLine("- " + item.Name);
                    }
                    Console.WriteLine("Choose one of those and try again.");
                } else
                {
                    var templateDir = await _templateHandler.CreateTemplate(args.Skip(1).ToList());
                    MaybeShowFileExplorer(templateDir);
                }
            }
            else
            {
                var names = await templateNames;
                if (names.Any(x => x.Name == firstAarg))
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

        private static void MaybeShowFileExplorer(string templateDir)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Process.Start("explorer.exe", "/select, " + templateDir);
            } else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", "-R " + templateDir);
            }
        }
        
        private static async Task ShowHelp(Task<IList<TemplateInfo>> templateNames, IList<TemplateInfo> resolvedNames = null)
        {

            Console.WriteLine("ZGen Code Generator");
            Console.WriteLine(@"
Options:
    gen <GENERATOR LETTER> - Generates a new template with default content and opens it in your default editor
    <template name> - executes that template

Supported generator letters:
- z : the default template source generator using z<NUMBER> variables.

If template name is the first argument, the following command line arguments are passed to the template:
    <template name> <arg1> <arg2> <arg3> ...

For the z generator:
Use z<NUMBER> anywhere in your template to mark a variable.
For example:
    z1 will be replaced with the first argument passed to the template
    z2 will be replaced with the second argument passed to the template
    z3 will be replaced with the third argument passed to the template
    ...
 
The variable can be placed in:
-   Directory name
-   File name
-   File content

If it is placed in the file content, the variable must be surrounded with {{ and }}
Like this: {{z42}}


Available templates:
");
            IList<TemplateInfo> names;
            if (resolvedNames != null)
                names = resolvedNames;
            else
                names = await templateNames;
            if (names != null && names.Count > 0)
            {
                names.ToList().ForEach(t => Console.WriteLine(t.Name + " -> " + t.Path));
            }
            else
            {
                Console.WriteLine("No templates found");
                Console.WriteLine("Create one with the 'gen' command");
            }
        }


    }
}
