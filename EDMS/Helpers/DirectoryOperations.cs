using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace EDMS.Helpers
{
    public class DirectoryOperations
    {
        /**
         *@params string: Path to the Directory to be created
         *@Returns Directory Creation Time
         **/
        public static bool createDirectory(string filePath)
        {
            try
            {
                DirectoryInfo newDirectory = Directory.CreateDirectory(filePath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}