# ImageResizer.Plugins.SitefinityProvider
Virtual image provider for Sitefinity CMS.

## Description
The plugin intersects requests to sitefinity image and serves them to ImageResizer system.
Plugin also supports query string parameter Status for returning proper lifecycle version of image.
If there is "tmb-" (LibrariesConfig.ThumbnailExtensionPrefix) in path, then it parses profile name and sends that name to presets plugin.
If there is no thumbnail name provided (eg: only tmb- in link), thumbnail name small is assumed.

## Example
Thumbnail name from url /images/default-source/first-page/image.tmb-thumb30.jpg would be thumb30

## Installation
Just install NuGet package and it will register pligon for ImageResizer in order to serve and process images.
 
