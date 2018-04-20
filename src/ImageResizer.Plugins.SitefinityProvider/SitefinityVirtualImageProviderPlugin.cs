using ImageResizer.Plugins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using ImageResizer.Configuration;
using System.Text;
using ImageResizer;

namespace ImageResizer.Plugins.SitefinityProvider
{
    public class SitefinityVirtualImageProviderPlugin : IVirtualImageProvider, IPlugin, IQuerystringPlugin, ISettingsModifier
    {
        #region IPlugin
        public IPlugin Install(Config c)
        {
            config = c;
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }
        private Config config;
        #endregion

        #region IQuerystringPlugin
        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return queryStringNames;
        }
        static string[] queryStringNames = new string[] { "Status", "sfvrsn" };
        #endregion

        #region IVirtualImageProvider
        public bool FileExists(string virtualPath, NameValueCollection queryString)
        {
            SitefinityVirtualImage image = EnsureImage(HttpContext.Current, virtualPath, queryString);
            return image != null && image.Image != null;
        }

        public IVirtualFile GetFile(string virtualPath, NameValueCollection queryString)
        {
            SitefinityVirtualImage image = EnsureImage(HttpContext.Current, virtualPath, queryString);
            if(image != null && image.Image != null)
            {
                return image;
            }
            return null;
        }

        #endregion

        public ResizeSettings Modify(ResizeSettings settings)
        {
            var options = EnsureImage(HttpContext.Current);
            if (options.ThumbnailProfile != null)
            {
                settings["preset"] = options.ThumbnailProfile.Name;
            }
            return settings;
        }

        #region sitefinity helper methods       

        /// <summary>
        /// Reads image from cache if exists. Otherwise it construct new one.       
        /// If http contenxt exists cache is saved in http context.         
        /// </summary>
        /// <returns></returns>
        private SitefinityVirtualImage EnsureImage(HttpContext httpContext)
        {
            return EnsureImage(httpContext, httpContext.Request.Url.AbsolutePath, httpContext.Request.QueryString);
        }

        /// <summary>
        /// Reads image from cache if exists. Otherwise it construct new one by resolving from virtualPath and querystring.
        /// If http contenxt exists cache is saved in http context.         
        /// </summary>
        /// <returns></returns>
        private SitefinityVirtualImage EnsureImage(HttpContext httpContext, string virtualPath, NameValueCollection queryString)
        {
            SitefinityVirtualImage result;
            if(httpContext != null)
            {
                result = httpContext.Items["SitefinityVirtualImageProviderPlugin_LoadedImage"] as SitefinityVirtualImage;
                if (result != null)
                {
                    return result;
                }
            }

            try
            {
                result = SitefinityHelpers.ResolveVirtualImage(virtualPath, queryString);
            }
            catch (Exception)
            {
                result = new SitefinityVirtualImage();
            }

            if (httpContext != null)
            {
                httpContext.Items["SitefinityVirtualImageProviderPlugin_LoadedImage"] = result;
            }

            return result;
        }
        #endregion
    }


}