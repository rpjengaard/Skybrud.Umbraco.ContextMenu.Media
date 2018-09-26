using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using ICSharpCode.SharpZipLib.Zip;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.ContextMenu.Media.Controllers.BackOffice
{
	[PluginController("DownloadMediaFolder")]
	public class DownloadMediaFolderController : UmbracoAuthorizedApiController
	{
		[HttpGet]
		public void GetMedia(int mediaFolderId, bool recursive = false)
		{
			var folderPropertyAlias = "folder";
			var response = HttpContext.Current.Response;

			// Fetch mediafolder
			var mediaFolder = UmbracoContext.MediaCache.GetById(mediaFolderId);

			if (mediaFolder == null)
				return;

			// Fetch content
			var files = recursive
				? mediaFolder.Descendants().ToList()
					.Where(x => x.ContentType.Alias.ToLower() != folderPropertyAlias)
				: mediaFolder.Children.ToList()
					.Where(x => x.ContentType.Alias.ToLower() != folderPropertyAlias);


			// Set content headers
			response.ContentType = "application/zip";
			response.AppendHeader("content-disposition", $"attachment; filename=\"{mediaFolder.Name}.zip\"");
			response.CacheControl = "Private";
			var zipOutputStream = new ZipOutputStream(response.OutputStream);

			// Collect and download files
			foreach (var file in files)
			{
				var fileUrl = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}{file.Url}";
				var fileBytes = new byte[0];

				try
				{
					fileBytes = new WebClient().DownloadData(fileUrl);
				}
				catch (WebException wex)
				{
					// if file returns 404, skip it
					if (((HttpWebResponse) wex.Response).StatusCode == HttpStatusCode.NotFound) continue;
				}


				// add file to zip
				if (fileBytes.Length > 0)
				{
					var fileEntry = new ZipEntry(file.Name)
					{
						Size = fileBytes.Length,
						DateTime = file.CreateDate
					};

					zipOutputStream.PutNextEntry(fileEntry);
					zipOutputStream.Write(fileBytes, 0, fileBytes.Length);
				}
			}

			zipOutputStream.Close();
			response.Flush();
			response.End();
		}
	}
}