using System;
using System.Collections.Generic;
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
                return path.Replace('/', '\\');
            }
            else
            {
                return path.Replace('\\', '/');
            }
        }
    }
}
