using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Common.Exceptions
{
    public class GeneralException : Exception
    {
        public GeneralException(string message) : base(message)
        {
            
        }
    }
}
