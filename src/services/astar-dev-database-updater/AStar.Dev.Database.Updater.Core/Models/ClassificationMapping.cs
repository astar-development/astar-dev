﻿namespace AStar.Dev.Database.Updater.Core.Models;

/// <summary>
///     The <see cref="ClassificationMapping" /> class
/// </summary>
public class ClassificationMapping
{
    /// <summary>
    ///     Gets or sets a section of text that should exist in the filename for this mapping to apply
    /// </summary>
    public string FileNameContains { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the text to store in the database for this mapping
    /// </summary>
    public string DatabaseMapping { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets whether this mapping is for someone no-one knows... joke!
    /// </summary>
    public bool Celebrity { get; set; }
}
