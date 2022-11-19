using System;
using System.Collections.Generic;
using System.Text;

namespace ZGenCodeGenerator.Exceptions
{
    public class TemplateAlreadyExistException : Exception
    {
        public TemplateAlreadyExistException(string message) : base(message)
        {
        }
    }

}
