using System;
using System.Collections.Generic;
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
            Console.WriteLine("Created template in " + templateDir);
        }

        public async Task Generate(string templateDir,
            string targetDir, Dictionary<string, string> varnames, IList<string> args)
        {
            targetDir = await EnsureGenerate(templateDir, targetDir);
            
            await TraverseDown(templateDir,
                dir =>
                {
                    var newDir = dir.Replace(templateDir, targetDir);
                    var translatedDir = TranslateDirOrFilePath(newDir, varnames);
                    _fileHandler.AddDirectory(translatedDir);
                    return translatedDir;
                },
                f =>
                {
                    var newPath = f.Replace(templateDir, targetDir);
                    var translatedFile = TranslateDirOrFilePath(newPath, varnames);
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

        private string TranslateDirOrFilePath(string dir, Dictionary<string, string> varnames)
        {
            var dirName = Path.GetFileName(dir);
            var parentDir = Path.GetDirectoryName(dir);
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

        public async Task<IEnumerable<string>> GetTemplateVariables(string templateDir)
        {
            if (!_fileHandler.DirectoryExist(templateDir))
            {
                throw new Exception("Template directory is empty");
            }
            HashSet<string> vars = new HashSet<string>();
            await TraverseDown(templateDir,
                dir => { AddVars(dir, vars); return dir; },
                file => { AddVars(file, vars); return file; },
                content => { AddVarsFromContent(content, vars); return content; }
                );
            var l = vars.OrderBy(x => x).ToList();
            return l;
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
