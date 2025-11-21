using System;

namespace AspnetCore_Changed_Files.Models;

public class ErrorViewModel
{
    public required string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}