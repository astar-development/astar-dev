using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;

namespace AspnetCore_Changed_Files.Models;

public class IndexViewModel
{
    public required IReadOnlyList<DriveItem> Items { get; set; }
    public required string DeltaToken { get; set; }
}
