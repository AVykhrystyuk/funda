using System.Diagnostics;

namespace Funda.Core.Models;

[DebuggerDisplay("Total = {Total} | Fetched = {Fetched}")]
public class ProgressInfo
{
    public long Total { get; set; } = -1;
    public long Fetched { get; set; } = -1;
}
