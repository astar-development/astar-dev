using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Configurations;

/// <summary>
/// </summary>
public static class ComplexPropertyBuilderConfiguration
{
    extension<TEntity>(ComplexPropertyBuilder<TEntity> propertyBuilder) where TEntity : notnull
    {
        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public ComplexPropertyBuilder<TEntity> Configure(IComplexPropertyConfiguration<TEntity> configuration)
        {
            configuration.Configure(propertyBuilder);

            return propertyBuilder;
        }
    }
}
