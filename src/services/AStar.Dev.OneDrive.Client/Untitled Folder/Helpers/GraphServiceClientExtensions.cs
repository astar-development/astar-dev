using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Graph.Drives.Item.Items.Item.Delta;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;

namespace AspnetCore_Changed_Files.Helpers
{
    public static class GraphServiceClientExtensions
    {
         private static readonly Regex TokenRegex = new Regex(@"[&?]token=(?<t>[^&]+)|token='(?<t>[^']+)");

         public static async Task<DeltaFiles> GetRootFilesAsync(this GraphServiceClient client, string deltaToken)
        {
            // 1. Get the Drive object first
            Drive? drive = await client.Me.Drive.GetAsync();

            if (drive == null) return null!;

            // 2. Create the request builder for the delta query
            // Path: /drives/{drive-id}/items/root/delta
            DeltaRequestBuilder? deltaRequestBuilder = client.Drives[drive.Id].Items["root"].Delta;
            
#pragma warning disable CS0618 // Type or member is obsolete
            Microsoft.Graph.Drives.Item.Items.Item.Delta.DeltaResponse result;
#pragma warning restore CS0618 // Type or member is obsolete

            // 3. Execute the query
            if (string.IsNullOrEmpty(deltaToken))
            {
                #pragma warning disable CS8600
                // Initial Request: Standard GetAsync
#pragma warning disable CS0618 // Type or member is obsolete
                result = await deltaRequestBuilder.GetAsync(config =>
                {
                    config.QueryParameters.Select = new[] { "id", "name", "lastModifiedDateTime", "folder", "webUrl", "deleted" }!;
                });
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
                // Subsequent Request: Create request info and inject the token manually
                RequestInformation requestInfo = deltaRequestBuilder.ToGetRequestInformation(config => 
                {
                    config.QueryParameters.Select = new[] { "id", "name", "lastModifiedDateTime", "folder", "webUrl", "deleted" };
                });
                
                // Manually add the token to the query string
                var uriBuilder = new UriBuilder(requestInfo.URI);
                NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = deltaToken;
                uriBuilder.Query = query.ToString();
                requestInfo.URI = uriBuilder.Uri;

                // Send the request using the modified URL
#pragma warning disable CS0618 // Type or member is obsolete
                result = await client.RequestAdapter.SendAsync(requestInfo, Microsoft.Graph.Drives.Item.Items.Item.Delta.DeltaResponse.CreateFromDiscriminatorValue);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS8600
            }

            // 4. Extract the new delta token/link
            var newDeltaLink = result?.OdataDeltaLink;
            string newDeltaToken = null!;

            if (!string.IsNullOrEmpty(newDeltaLink))
            {
                Match match = TokenRegex.Match(newDeltaLink);
                if (match.Success) newDeltaToken = match.Groups["t"].Value;
            }

            // 5. Return results
            return new DeltaFiles
            {
                Files = result?.Value?.Where(r => r.Folder == null).OrderByDescending(r => r.LastModifiedDateTime).ToList() ?? new List<DriveItem>(),
                DeltaToken = newDeltaToken
            };
        }
    }

    public class DeltaFiles
    {
        public required string DeltaToken { get; set; }

        public required IReadOnlyList<DriveItem> Files { get; set; }
    }
}
