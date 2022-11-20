using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZGenCodeGenerator.FileHandling;

namespace ZGenCodeGenerator.generators
{
    public class ZGenerator : GeneratorBase, IGenerator
    {

        public ZGenerator(IFileHandler fileHandler) : base(fileHandler)
        {
        }
        
        public async Task CreateTemplate(string templateDir)
        {
            await _fileHandler.EnsureDirExist(templateDir);
            var dir = Path.Combine(templateDir, "z1");
            await _fileHandler.AddDirectory(dir);
            var file = Path.Combine(dir, "z2.txt");
            await _fileHandler.WriteAllTextAsync(file, "hello {{z1}}");
            Console.WriteLine("Created template in " + templateDir +
                "\nwith 2 parameters");
        }

        public async Task Generate(string templateDir,
            string targetDir, Dictionary<string, string> varnames)
        {
            if (varnames == null)
                throw new ArgumentNullException(nameof(varnames));
            targetDir = await EnsureGenerate(templateDir, targetDir);
            
            await TraverseDown(templateDir,
                dir =>
                {
                    var newDir = dir.Replace(templateDir, targetDir);
                    var translatedDir = TranslateDirOrFilePath(targetDir, newDir, varnames);
                    _fileHandler.AddDirectory(translatedDir).GetAwaiter().GetResult();
                    return translatedDir;
                },
                f =>
                {
                    var newPath = f.Replace(templateDir, targetDir);
                    var translatedFile = TranslateDirOrFilePath(targetDir, newPath, varnames);
                    return translatedFile;
                },
                content =>
                {
                    var translatedContent = TranslateContent(content, varnames);
                    return translatedContent;
                }
                );

        }

        private string TranslateContent(string content, Dictionary<string, string> varnames)
        {
            return _recognizeZTemplVarContent.Replace(content, m =>
            {
                var varName = m.Groups[1].Value;
                if (varnames.ContainsKey(varName))
                {
                    return varnames[varName];
                }
                else
                {
                    return m.Value;
                }
            });
        }

        private string TranslateDirOrFilePath(string targetDir, string dir, Dictionary<string, string> varnames)
        {
            var dirName = Path.GetFileName(dir);
            var parentDir = Path.GetDirectoryName(dir);
            if(parentDir.Length > targetDir.Length)
            {
                parentDir = TranslateDirOrFilePath(targetDir, parentDir, varnames);
            }
            var translated = _recognizeZTemplVar.Replace(dirName, m =>
            {
                var varName = m.Groups[1].Value;
                if (varnames.ContainsKey(varName))
                {
                    return varnames[varName];
                }
                else
                {
                    return m.Value;
                }
            });
            var fullDir = Path.Combine(parentDir, translated);
            return fullDir;
        }

        public async Task<IList<string>> GetTemplateVariables(string templateDir)
        {
            if (!templateDir.Contains(Path.DirectorySeparatorChar))
            {
                templateDir = await _fileHandler.GetTemplatePath(templateDir);
            }
            
            if (!_fileHandler.DirectoryExist(templateDir))
            {
                throw new Exception("Template directory does not exist");
            }
            HashSet<string> vars = new HashSet<string>();
            await TraverseDown(templateDir,
                dir => { AddVars(dir, vars); return dir; },
                file => { AddVars(file, vars); return file; },
                content => { AddVarsFromContent(content, vars); return content; }
                );
            var ldict = vars.ToDictionary(v => int.Parse(v.Substring(1)), k => k );
            var sorted = ldict.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
            return sorted;
        }

        private static Regex _recognizeZTemplVar = new Regex("(z[0-9]+)", RegexOptions.Compiled);
        private static Regex _recognizeZTemplVarContent = new Regex("{{(z[0-9]+)}}", RegexOptions.Compiled);

        private void AddVarsFromContent(string content, HashSet<string> vars)
        {
            _recognizeZTemplVarContent.Matches(content).ToList().ForEach(x =>
            {
                vars.Add(x.Groups[1].Value);
            });
        }

        private void AddVars(string dir, HashSet<string> vars)
        {
            _recognizeZTemplVar.Matches(dir).ToList().ForEach(x =>
            {
                vars.Add(x.Groups[1].Value);
            });
        }

        private async Task<string> EnsureGenerate(string templateDir, string targetDir)
        {
            targetDir = await GetDirOrCurrent(targetDir);
            if (!_fileHandler.DirectoryExist(templateDir))
            {
                throw new Exception("Template directory is empty");
            }
            if (!_fileHandler.DirectoryExist(targetDir))
            {
                await _fileHandler.AddDirectory(targetDir);
            }

            return targetDir;
        }

        private async Task<string> GetDirOrCurrent(string targetDir)
        {
            if (targetDir == null)
            {
                targetDir = await _fileHandler.GetCurrDir();
            }

            return targetDir;
        }

        public IFileHandler GetFileHandler()
        {
            return _fileHandler;
        }

    }
}
