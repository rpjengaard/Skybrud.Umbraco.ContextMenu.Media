using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Skybrud.Umbraco.ContextMenu.Media.EventHandlers
{
	public class AddMediaDownloadToContextMenu : ApplicationEventHandler
	{
		public AddMediaDownloadToContextMenu()
		{
			TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;
		}

		private void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
		{
			var treeAlias = sender.TreeAlias;
			var dialogViewPath = "/App_Plugins/Skybrud.Umbraco.ContextMenu.Media/Views/downloadMediaFolder.html";
			var title = "Download Media Folder";


			if (treeAlias.ToLower() == "media")
			{
				var nodeId = 0;
				int.TryParse(e.NodeId, out nodeId);

				var media = sender.Services.MediaService.GetById(nodeId);

				if (media != null && !media.Trashed)
					if (media.ContentType.Alias.ToLower() == "folder")
					{
						var downloadItem = new MenuItem();

						downloadItem.Alias = "downloadMediaFolder";
						downloadItem.Icon = "page-down";
						downloadItem.Name = "Download folder"; //TODO: translate with localization
						downloadItem.SeperatorBefore = true;
						downloadItem.LaunchDialogView(dialogViewPath, title);


						//e.Menu.Items.Add(downloadItem);
						e.Menu.Items.Insert(e.Menu.Items.Count - 1, downloadItem); //place just before "Reload nodes"
					}
			}
		}
	}
}