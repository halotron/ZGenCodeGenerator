using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenCodeGenerator.FileHandling
{
    public class FileHandler : IFileHandler
    {
        public Task AddDirectory(string dir)
        {
            Directory.CreateDirectory(dir);
            return Task.CompletedTask;
        }

        public bool DirectoryExist(string dir)
        {
            return Directory.Exists(dir);
        }

        public async Task EnsureDirExist(string dir)
        {
            if (!Directory.Exists(dir))
            {
                await EnsureDirExist(Path.GetDirectoryName(dir));
                Directory.CreateDirectory(dir);
            }
        }

        public Task<string> GetCurrDir()
        {
            return Task.FromResult(Directory.GetCurrentDirectory());
        }

        public IList<string> GetDirectories(string dir)
        {
            return Directory.GetDirectories(dir)?.ToList();
        }

        public async Task<string> GetExistingTemplateDir()
        {
            var dir = Directory.GetCurrentDirectory();
            string pathDir = null;
            return await Task.Run(async () =>
            {
                await TraverseTemplates(dir,
                                x =>
                                {
                                    if (pathDir == null)
                                    {
                                        pathDir = Path.GetDirectoryName(x);
                                    }
                                    return true;
                                },
                                () => null
                                );
                return pathDir;
            }
            );
        }

        public IList<string> GetFiles(string dir)
        {
            return Directory.GetFiles(dir)?.ToList();
        }

        public string GetTemplateDirName()
        {
            return ".zgentemplates";
        }

        public async Task<IList<string>> GetTemplateNames()
        {
            var dir = Directory.GetCurrentDirectory();
            return await Task.Run(async () =>
            {
                var set = new HashSet<string>();
                return await TraverseTemplates(dir,
                    x => set.Add(Path.GetFileName(x)),
                    () => set.ToList()
                    );
            }
            );
        }


        public async Task<string> GetTemplatePath(string templateName)
        {
            var dir = Directory.GetCurrentDirectory();
            string pathDir = null;
            return await Task.Run(async () =>
            {
                await TraverseTemplates(dir,
                                x =>
                                {
                                    var fn = Path.GetFileName(x).ToLower();
                                    if (fn == templateName)
                                    {
                                        pathDir = x;
                                    }
                                    return true;
                                },
                                () => null
                                );
                return pathDir;
            }
            );
        }

        public async Task<string> ReadAllTextAsync(string f)
        {
            return await File.ReadAllTextAsync(f);
        }

        public async Task WriteAllTextAsync(string newFile, string newContent)
        {
            await File.WriteAllTextAsync(newFile, newContent);
        }

        private async Task<IList<string>> TraverseTemplates(string dir,
            Func<string, bool> onTemplateFound, Func<IList<string>> onDone)
        {
            var tmplPath = Path.Combine(dir, GetTemplateDirName());
            if (Directory.Exists(tmplPath))
            {
                var subDirs = Directory.GetDirectories(tmplPath);
                foreach (var item in subDirs)
                {
                    var templateNames = Directory.GetDirectories(item);
                    foreach (var templateName in templateNames)
                    {
                        onTemplateFound(templateName);
                    }
                }
            }
            var parentDir = Directory.GetParent(dir);
            if (parentDir != null)
            {
                return await TraverseTemplates(parentDir.FullName, onTemplateFound, onDone);
            }
            else
            {
                return onDone();
            }
        }

    }
}
