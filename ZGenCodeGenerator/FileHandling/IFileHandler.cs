using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZGenCodeGenerator.Models;

namespace ZGenCodeGenerator.FileHandling
{
    public interface IFileHandler
    {
        string PathCombine(string path1, string path2);
        char PathDirectorySeparatorChar { get; }
        Task<IList<TemplateInfo>> GetTemplateInfos();
        Task<string> GetTemplatePath(string templateName);
        Task<string> GetExistingTemplateDir();
        Task<string> GetCurrDir();
        Task EnsureDirExist(string dir);
        string GetTemplateDirName();
        IList<string> GetFiles(string dir);
        IList<string> GetDirectories(string dir);
        Task AddDirectory(string dir);
        bool DirectoryExist(string dir);
        Task<string> ReadAllTextAsync(string f);
        Task WriteAllTextAsync(string newFile, string newContent);
    }
}
