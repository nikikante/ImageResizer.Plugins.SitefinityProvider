using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Libraries.Configuration;
using Telerik.Sitefinity.GenericContent.Model;

namespace ImageResizer.Plugins.SitefinityProvider
{
    class SitefinityVirtualImage : IVirtualFile
    {
        public SitefinityVirtualImage()
        {

        }

        public SitefinityVirtualImage(Telerik.Sitefinity.Libraries.Model.Image image, LibrariesManager manager)
        {
            this.Image = image;
            this.Manager = manager;
        }

        public string VirtualPath
        {
            get
            {
                if(Image == null)
                {
                    throw new InvalidOperationException("Cannot read virtual path, because image is not loaded!");
                }
                return Image.Url;
            }
        }

        public Stream Open()
        {
            if (Image == null)
            {
                throw new InvalidOperationException("Cannot read file, because image is not loaded!");
            }
            if (Manager == null)
            {
                throw new InvalidOperationException("Cannot read file, because LibrariesManager is not set!");
            }
            return Manager.Download(Image);
        }

        public Telerik.Sitefinity.Libraries.Model.Image Image;
        public LibrariesManager Manager;
        public ThumbnailProfileConfigElement ThumbnailProfile;
    }
}
