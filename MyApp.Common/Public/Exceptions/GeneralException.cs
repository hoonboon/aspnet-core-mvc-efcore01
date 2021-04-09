using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Common.Public.Exceptions
{
    public class GeneralException : Exception
    {
        public GeneralException(string message) : base(message)
        {

        }
    }
}
