using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenCodeGenerator.Tests
{
    public class BaseTest
    {
        public string CrossPath(string path)
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                if (path.Contains("/"))
                {
                    Debug.WriteLine("Path contains / : " + path);
                    path = path.Replace("/", "\\");
                }
                return path;
            }
            else
            {
                if (path.Contains("\\"))
                {
                    Debug.WriteLine("Path contains \\ : " + path);
                    path = path.Replace("\\", "/");
                }
                return path;
            }
        }
    }
}
