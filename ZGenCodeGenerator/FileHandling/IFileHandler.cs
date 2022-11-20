using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZGenCodeGenerator.Models;

namespace ZGenCodeGenerator.FileHandling
{
    public interface IFileHandler
    {
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
