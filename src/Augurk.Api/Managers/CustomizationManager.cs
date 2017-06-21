using System.Security.Cryptography;
using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve customization settings from storage.
    /// </summary>
    public class CustomizationManager
    {
        private const string KEY = "urn:Augurk:Customization";

        /// <summary>
        /// Retrieves the customization settings; or, creates them if they do not exist.
        /// </summary>
        /// <returns>A <see cref="Customization"/> instance containing the customization settings.</returns>
        public async Task<Customization> GetOrCreateCustomizationSettingsAsync()
        {
            Customization customizationSettings = null;

            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                customizationSettings = await session.LoadAsync<Customization>(KEY);
            }

            if (customizationSettings == null)
            {
                customizationSettings = new Customization {InstanceName = "Augurk"};
                await PersistCustomizationSettingsAsync(customizationSettings);
            }

            return customizationSettings;
        }

        /// <summary>
        /// Persists the provided customization settings.
        /// </summary>
        /// <param name="customizationSettings">A <see cref="Customization"/> instance containing the settings that should be persisted.</param>
        public async Task PersistCustomizationSettingsAsync(Customization customizationSettings)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                // Using the store method when the product already exists in the database will override it completely, this is acceptable
                await session.StoreAsync(customizationSettings, KEY);
                await session.SaveChangesAsync();
            }
        }
    }
}
