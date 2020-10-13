using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace EDMS.Helpers
{
    public class FileOperations
    {
        public static bool saveFileToServer(HttpPostedFileBase file, string filePath)
        {
            try
            {
                //string path = Server.MapPath(filePath);
                string pathString = System.IO.Path.Combine(filePath, file.FileName);

                /*if (!Directory.Exists(filePath))
                {
                    return false;
                }*/

                file.SaveAs(pathString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}