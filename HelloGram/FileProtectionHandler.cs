using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace HelloGram
{
    public class FileProtectionHandler : IHttpHandler
    {
        public bool IsReusable { get; }

        public FileProtectionHandler(bool isReusable)
        {
            IsReusable = isReusable;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod != "GET") return;

            // Is the user logged-in?
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.Redirect("~/Views/Shared/NotFound.cshtml");
            }
            else
            {
                var requestedFile = context.Server.MapPath(context.Request.FilePath);
                SendContentTypeAndFile(context, requestedFile);
            }

            //var requestedFile = context.Server.MapPath(context.Request.FilePath);
            //// Verify the user has access to the User role.
            //if (context.User.IsInRole("User"))
            //{
            //    SendContentTypeAndFile(context, requestedFile);
            //}
            //else
            //{
            //    // Deny access, redirect to error page or back to login page.
            //    context.Response.Redirect("~/Views/Shared/NotFound.cshtml");
            //}
        }

        private static void SendContentTypeAndFile(HttpContext context, string strFile)
        {
            context.Response.ContentType = GetContentType(strFile);
            context.Response.TransmitFile(strFile);
            context.Response.End();
        }

        private static string GetContentType(string filename)
        {
            // used to set the encoding for the response stream
            string res = null;
            var fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists)
            {
                return null;
            }

            if (fileInfo.Extension.Remove(0, 1).ToLower().Equals("pdf"))
            {
                res = "application/pdf";
            }

            return res;

        }
    }
}