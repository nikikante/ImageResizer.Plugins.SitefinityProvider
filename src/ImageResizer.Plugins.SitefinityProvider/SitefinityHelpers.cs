using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Libraries.Configuration;
using Telerik.Sitefinity.GenericContent.Model;
using System.Collections.Specialized;

namespace ImageResizer.Plugins.SitefinityProvider
{
    class SitefinityHelpers
    {

        public static Telerik.Sitefinity.Libraries.Model.Image ProcessStatuses(LibrariesManager manager, Telerik.Sitefinity.Libraries.Model.Image image, string status)
        {
            if (image == null)
            {
                return null;
            }

            #region return right version of item
            bool isStatusSet = false;
            if (!string.IsNullOrEmpty(status))
            {
                ContentLifecycleStatus itemStatus;
                if (Enum.TryParse<ContentLifecycleStatus>(status, out itemStatus))
                {
                    switch (itemStatus)
                    {
                        case ContentLifecycleStatus.Master:
                            if (image.Status != ContentLifecycleStatus.Master)
                            {
                                image = (Telerik.Sitefinity.Libraries.Model.Image)manager.Lifecycle.GetMaster(image);
                                isStatusSet = true;
                            }
                            break;

                        case ContentLifecycleStatus.Temp:
                            if (image.Status != ContentLifecycleStatus.Temp)
                            {
                                image = (Telerik.Sitefinity.Libraries.Model.Image)manager.Lifecycle.GetTemp(image);
                                isStatusSet = true;
                            }
                            break;

                        case ContentLifecycleStatus.Live:
                            if (image.Status != ContentLifecycleStatus.Live)
                            {
                                image = (Telerik.Sitefinity.Libraries.Model.Image)manager.Lifecycle.GetLive(image);
                                isStatusSet = true;
                            }
                            break;
                        default:
                            if (itemStatus != ContentLifecycleStatus.Deleted && image.Status != ContentLifecycleStatus.Live)
                            {
                                image = (Telerik.Sitefinity.Libraries.Model.Image)manager.Lifecycle.GetLive(image);
                                isStatusSet = true;
                            }
                            break;
                    }
                }
            }
            if (!isStatusSet)
            {
                if (image.Status != ContentLifecycleStatus.Live)
                {
                    image = (Telerik.Sitefinity.Libraries.Model.Image)manager.Lifecycle.GetLive(image);
                    isStatusSet = true;
                }
            }
            #endregion
            return image;
        }


        public static string ExtractProviderName(string url)
        {
            url = url.Substring(8);
            int indexOfSlash = url.IndexOf("/");
            if (indexOfSlash >= 0)
            {
                return url.Substring(0, indexOfSlash);
            }
            throw new HttpException("Can process requested image! NOPROVIDER");
        }

        public static string ExtractSitefinityMediaName(string url, LibrariesConfig config, out string thumbnailProfileName)
        {
            int indexOfDot = url.LastIndexOf('.');
            if (indexOfDot >= 0)
            {
                url = url.Substring(0, indexOfDot - 1);
            }
            thumbnailProfileName = "";
            var thumbnailExtensionPrefix = config.ThumbnailExtensionPrefix;
            if (url.EndsWith(thumbnailExtensionPrefix))
            {
                url = url.Left(url.Length - thumbnailExtensionPrefix.Length);
                thumbnailProfileName = "small";
            }
            else
            {
                foreach (var thumbnailProfile in config.Images.Thumbnails.Profiles.OfType<ThumbnailProfileConfigElement>())
                {

                    if (url.EndsWith(thumbnailExtensionPrefix + thumbnailProfile.Name))
                    {
                        url = url.Left(url.Length - thumbnailProfile.Name.Length - thumbnailExtensionPrefix.Length);
                        thumbnailProfileName = thumbnailProfile.Name;
                        break;
                    }
                }
            }
            return url;
        }


        public static SitefinityVirtualImage ResolveVirtualImage(string virtualPath, NameValueCollection queryString)
        {
            var requestUrl = virtualPath + "?" + HttpUtilities.QueryStringSerialize(queryString);
            if (!requestUrl.StartsWith("/images/"))
            {
                throw new HttpException("Can process requested image! NOIMAGEDIR");
            }

            var result = new SitefinityVirtualImage();
            LibrariesManager manager;

            var librariesConfig = Telerik.Sitefinity.Configuration.Config.Get<LibrariesConfig>();
            #region branje thumbnail profila iz url-ja
            string thumbnailProfileName;
            string sitefinityNormalizedImagePath = ExtractSitefinityMediaName(requestUrl, librariesConfig, out thumbnailProfileName);
            result.ThumbnailProfile = librariesConfig.Images.Thumbnails.Profiles[thumbnailProfileName];
            #endregion

            #region branje provider name iz url-ja
            string providerName = ExtractProviderName(requestUrl);
            var providerConfig = librariesConfig.Providers.Elements.FirstOrDefault(a => a.GetParameter("urlName") == providerName);
            manager = providerConfig != null ? new LibrariesManager(providerConfig.Name) : new LibrariesManager();
            #endregion

            string redirectUrl;
            var image = manager.GetItemFromUrl<Telerik.Sitefinity.Libraries.Model.Image>(sitefinityNormalizedImagePath, out redirectUrl);
            result.Image = ProcessStatuses(manager, image, queryString["Status"]);
            result.Manager = manager;

            return result;
        }
    }
}
