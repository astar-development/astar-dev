using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCore_Changed_Files.Extensions
{
    public class AzureAdOptions
    {
        public required string ClientId { get; set; }

        public required string ClientSecret { get; set; }

        public required string Instance { get; set; }

        public required string Domain { get; set; }

        public required string TenantId { get; set; }

        public required string CallbackPath { get; set; }

        public required string BaseUrl { get; set; }

        public required string Scopes { get; set; }

        public required string GraphResourceId { get; set; }

        public required string GraphScopes { get; set; }
    }
}
