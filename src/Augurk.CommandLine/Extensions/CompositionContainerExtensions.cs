using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Contains extension methods for the MEF <see cref="CompositionContainer"/>.
    /// </summary>
    internal static class CompositionContainerExtensions
    {
        /// <summary>
        /// Adds the <paramref name="exportedValue"/> instance into the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">A <see cref="CompositionContainer"/> to inject the exported value into.</param>
        /// <param name="exportedValue">A value to inject into the <paramref name="container"/>.</param>
        public static void ComposeExportedValue(this CompositionContainer container, object exportedValue)
        {
            // Validate arguments
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (exportedValue == null)
                throw new ArgumentNullException(nameof(exportedValue));

            // Create the composition batch
            CompositionBatch batch = new CompositionBatch();
            var metadata = new Dictionary<string, object> {
                { "ExportTypeIdentity", AttributedModelServices.GetTypeIdentity(exportedValue.GetType()) }
            };

            // Add the exported value
            var contractName = AttributedModelServices.GetContractName(exportedValue.GetType());
            batch.AddExport(new Export(contractName, metadata, () => exportedValue));
            container.Compose(batch);
        }
    }
}
