using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZGenCodeGenerator.FileHandling;

namespace ZGenCodeGenerator.generators
{
    public class GeneratorBase
    {
        protected readonly IFileHandler _fileHandler;

        public GeneratorBase(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }
        
        protected async Task TraverseDown(string dir, Func<string, string> onDir,
            Func<string, string> onFile, Func<string, string> onFileContent)
        {
            var dirs = _fileHandler.GetDirectories(dir);
            if (dirs != null)
            {
                foreach (var d in dirs)
                {
                    var newDir = onDir(d);
                    if (d != null)
                    {
                        await this.TraverseDown(d, onDir, onFile, onFileContent);
                    }
                }
            }
            var files = _fileHandler.GetFiles(dir);
            if (files != null)
            {
                foreach (var f in files)
                {
                    var newFile = onFile(f);
                    if (newFile != null)
                    {
                        var content = await _fileHandler.ReadAllTextAsync(f);
                        var newContent = onFileContent(content);
                        if (newContent != null)
                        {
                            await _fileHandler.WriteAllTextAsync(newFile, newContent);
                        }
                    }
                }

            }        }
        
    }
}
