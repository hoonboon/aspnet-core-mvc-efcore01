using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Utils
{
    public static class Constants
    {
        public static readonly string SUCCESS_MESSAGE = "Request successfully processed.";
        
        public static readonly string ERROR_MESSAGE_SAVE = "Unable to save changes."
                    + " Please try again."
                    + " If the problem persists, please contact the system administrator.";
        
        public static readonly string ERROR_MESSAGE_DELETE = "Unable to delete the record."
                    + " Please try again."
                    + " If the problem persists, please contact the system administrator.";

    }
}
