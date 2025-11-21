using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;

namespace AStar.Dev.OneDrive.Client.Services
{
    public class OneDriveService
    {
        private readonly GraphServiceClient _client;

        public OneDriveService(GraphServiceClient client) => _client = client;

        /// <summary>
        /// Lists items in the user's OneDrive root folder.
        /// </summary>
        public async Task<List<DriveItem>> GetRootItemsAsync()
        {
            Drive? item = await _client.Me.Drive
                .GetAsync();
    
            List<DriveItem>? rootChildren = item?.Root?.Children;
            return rootChildren?.ToList() ?? [];
        }

        /// <summary>
        /// Get a DriveItem by its path relative to root.
        /// Example path: "/Documents/MyFile.txt"
        /// </summary>
        public async Task<DriveItem?> GetItemByPathAsync(string path)
        {
            Drive? item = await _client.Me.Drive
                .GetAsync();
    
            return item?.Items?.FirstOrDefault(i => i.Folder?.ToString() == path);
        }

        /// <summary>
        /// Download the content of a file at a given path
        /// </summary>
        public async Task<System.IO.Stream> DownloadFileAsync(string path)
        {
            Drive? item = await _client.Me.Drive
                .GetAsync();
            // var stream = item.
            //     .ItemWithPath(path)
            //     .Content
            //     .GetAsync();

            return null!;
        }

        /// <summary>
        /// Uploads a file to a specific path (overwrites if exists)
        /// </summary>
        public async Task<DriveItem> UploadFileAsync(string path, System.IO.Stream content)
        {
            await Task.CompletedTask;
            // var uploadedItem = await _client.Me.Drive
            //     .Special["root"]
            //     .ItemWithPath(path)
            //     .Content
            //     .PutAsync(content);

            return null!;
        }

        /// <summary>
        /// Creates a folder at a given path under root
        /// </summary>
        public async Task<DriveItem> CreateFolderAsync(string folderPath, string folderName)
        {
            await Task.CompletedTask;
            var folderToCreate = new DriveItem
            {
                Name = folderName,
                Folder = new Folder(),
                AdditionalData = new Dictionary<string, object>
                {
                    {"@microsoft.graph.conflictBehavior", "rename"}
                }
            };

            // var createdFolder = await _client.Me.Drive
            //     .Special["root"]
            //     .ItemWithPath(folderPath)
            //     .Children
            //     .PostAsync(folderToCreate);

            return null!;
        }
    }
}
