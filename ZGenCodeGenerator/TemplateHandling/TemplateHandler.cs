using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGenCodeGenerator.FileHandling;
using ZGenCodeGenerator.generators;
using ZGenCodeGenerator.Exceptions;

namespace ZGenCodeGenerator.TemplateHandling
{
    public class TemplateHandler : ITemplateHandler
    {
        private IFileHandler _fileHandler;
        private readonly Lazy<IGeneratorFactory> _generatorFactory;

        public TemplateHandler(IFileHandler fileHandler,
            Lazy<IGeneratorFactory> generatorFactory)
        {
            _fileHandler = fileHandler;
            _generatorFactory = generatorFactory;
        }


        public async Task CreateTemplate(IList<string> args)
        {
            IGenerator generator = null;
            bool firstTry = true;
            string line = null;
            string generatorName = null;
            do
            {

                if (args != null && args.Count > 0)
                {
                    line = args[0];
                    if (!firstTry)
                    {
                        throw new Exception($"No template type called {line}");
                    }
                }
                else
                {
                    Console.WriteLine("Which template do you want to generate?");
                    line = Console.ReadLine().ToLower().Trim();
                }
                firstTry = false;
                generator = _generatorFactory.Value.GetGenerator(line);
            } while (generator == null);
            generatorName = line;
            line = null;
            firstTry = true;
            do
            {

                if (args != null && args.Count > 1)
                {
                    line = args[1];
                    if (!firstTry)
                    {
                        throw new TemplateAlreadyExistException($"template can not be called {line}");
                    }
                }
                else
                {
                    Console.WriteLine("Name of template:");
                    line = Console.ReadLine().ToLower();
                }
                firstTry = false;
                var existing = await _fileHandler.GetTemplatePath(line);
                if (existing != null)
                {
                    line = null;
                }

            } while (line == null);
            string templateName = line;
            string templPath = null;
            string pathChoice = null;
            firstTry = true;
            do
            {
                if (args != null && args.Count > 2)
                {
                    pathChoice = MakeSurePathChoice(args[2]);
                }
                else
                {
                    Console.WriteLine("1 for current directory, 2 for closest directory with existing templates");
                    pathChoice = MakeSurePathChoice(Console.ReadLine().ToLower().Trim());
                }
                var existingDir = await _fileHandler.GetExistingTemplateDir();
                var currDir = await _fileHandler.GetCurrDir();
                if (!string.IsNullOrWhiteSpace(existingDir) && pathChoice == "2")
                {
                    templPath = existingDir;
                }
                else
                {
                    templPath = currDir;
                }
            } while (templPath == null);
            var templateDir = templPath +
                Path.DirectorySeparatorChar + generatorName + Path.DirectorySeparatorChar + templateName;

            await generator.CreateTemplate(templateDir);
        }

        private string MakeSurePathChoice(string choice)
        {
            string pathChoice;
            if (choice == "1")
            {
                pathChoice = "1";
            }
            else if (choice == "2")
            {
                pathChoice = "2";
            }
            else
            {
                throw new Exception($"path choice can not be called {choice}");
            }

            return pathChoice;
        }
        
        public async Task Generate(string templateName, IList<string> args)
        {
            string pathDir = await _fileHandler.GetTemplatePath(templateName);

            if (pathDir == null)
            {
                throw new ArgumentException($"No template called {templateName}");
            }

            var dirType = Path.GetDirectoryName(pathDir);
            var indexLast = dirType.LastIndexOf(Path.DirectorySeparatorChar);
            var typeOfTemplate = dirType.Substring(indexLast + 1);
            var generator = _generatorFactory.Value.GetGenerator(typeOfTemplate);

            var variables = await generator.GetTemplateVariables(pathDir);
            int nrVariables = variables?.Count ?? 0;
            int nrArgs = (args?.Count ?? 0);

            if (nrArgs != nrVariables)
            {
                if (nrArgs < nrVariables)
                {
                    throw new NumberOfVariableParametersNotMatchingException($"Not enough arguments for template {templateName}")
                    {
                        TemplateName = templateName,
                        Variables = variables,
                        ProvidedParameters = args?.Skip(2)?.ToList()
                    };
                }
                else
                {
                    throw new NumberOfVariableParametersNotMatchingException($"Too many arguments for template {templateName}")
                    {
                        TemplateName = templateName,
                        Variables = variables,
                        ProvidedParameters = args?.Skip(2)?.ToList()
                    };
                }
            }
            var dictArgs = new Dictionary<string, string>();
            for (int i = 0; i < nrVariables; i++)
            {
                dictArgs.Add(variables[i], args[i]);
            }

            var targetDir = await _fileHandler.GetCurrDir();
            await generator.Generate(pathDir, targetDir, dictArgs);
        }

    }
}
