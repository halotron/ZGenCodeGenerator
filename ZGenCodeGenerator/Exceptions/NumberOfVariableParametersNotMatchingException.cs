using System;
using System.Collections.Generic;
using System.Text;

namespace ZGenCodeGenerator.Exceptions
{
    public class NumberOfVariableParametersNotMatchingException : Exception
    {
        public NumberOfVariableParametersNotMatchingException(string message) : base(message)
        {
        }

        public string TemplateName { get; set; }

        public IEnumerable<string> Variables { get; set; }
        public IEnumerable<string> ProvidedParameters { get; set; }
    }

}
