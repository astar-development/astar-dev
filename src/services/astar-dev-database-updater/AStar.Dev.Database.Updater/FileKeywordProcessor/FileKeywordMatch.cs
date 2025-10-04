using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Database.Updater.FileKeywordProcessor;

public class FileKeywordMatch
{
    public int Id { get; set; }

    [MaxLength(300)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Keyword { get; set; } = string.Empty;
}
