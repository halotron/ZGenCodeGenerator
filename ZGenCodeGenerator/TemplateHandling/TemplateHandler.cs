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
        
        public async Task Generate(string firstAarg, IEnumerable<string> args)
        {
            var dir = Directory.GetCurrentDirectory();
            string pathDir = await _fileHandler.GetTemplatePath(firstAarg);

            var dirType = Path.GetDirectoryName(pathDir);
            var indexLast = dirType.LastIndexOf(Path.DirectorySeparatorChar);
            var typeOfTemplate = dirType.Substring(indexLast + 1);
            IGenerator generator = null;
            generator = _generatorFactory.Value.GetGenerator(typeOfTemplate);
            await generator.Generate(pathDir, null, null, args.ToList());
        }

    }
}
