namespace AStar.Dev.Database.Updater.Core.Files;

/// <summary>
///     The <see cref="ClassificationMappingExtensions" /> containing the ClassificationMapping extensions
/// </summary>
public static class ClassificationMappingExtensions
{
    // /// <summary>
    // ///     The ToFileClassification method maps the <see cref="ClassificationMapping" /> to <see cref="FileClassification" />
    // /// </summary>
    // /// <param name="mapping">The <see cref="ClassificationMapping" /> to map</param>
    // /// <returns>the mapped <see cref="FileClassification" /></returns>
    // public static FileClassification ToFileClassification(this ClassificationMapping mapping) =>
    //     new () { Name          = mapping.DatabaseMapping, Celebrity     = mapping.Celebrity == "TRUE", FileNameParts = [new() { Text = mapping.FileNameContains }] };
    //
    // /// <summary>
    // ///     The AddFileClassifications method will take the <see cref="ClassificationMapping" /> and add them to the <see cref="FilesContext" /> if they do not already exist
    // /// </summary>
    // /// <param name="classificationMappings">The sequence of <see cref="ClassificationMapping" /> to add to the context if not present</param>
    // /// <param name="filesContext">The instance of <see cref="FilesContext" /> to add the mappings to</param>
    // /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    // /// <remarks>Change to use the <see cref="Result{TSuccess,TFailure}" /> type</remarks>
    // public static bool AddFileClassifications(this IEnumerable<ClassificationMapping> classificationMappings, FilesContext filesContext)
    // {
    //     foreach (var mapping in classificationMappings)
    //     {
    //         var fileClassification = filesContext.FileClassifications
    //                                              .Include(f => f.FileNameParts)
    //                                              .FirstOrNone(x => x.Name == mapping.DatabaseMapping);
    //
    //         fileClassification.Map(existingFileClassification => UpdateExistingFileClassification(existingFileClassification, mapping))
    //                           .Reduce(() => AddClassificationToDatabase(mapping, filesContext));
    //
    //         filesContext.SaveChanges();
    //     }
    //
    //     return true;
    // }
    //
    // private static FileClassification AddClassificationToDatabase(ClassificationMapping mapping, FilesContext filesContext)
    // {
    //     var fileClassification = mapping.ToFileClassification();
    //     filesContext.FileClassifications.Add(fileClassification);
    //
    //     return fileClassification;
    // }
    //
    // private static FileClassification UpdateExistingFileClassification(FileClassification fileClassification, ClassificationMapping mapping)
    // {
    //     if (fileClassification.FileNameParts.All(d => !d.Text.Equals(mapping.FileNameContains, StringComparison.OrdinalIgnoreCase)))
    //     {
    //         fileClassification.FileNameParts.Add(new () { Text = mapping.FileNameContains });
    //     }
    //
    //     return fileClassification;
    // }
}
