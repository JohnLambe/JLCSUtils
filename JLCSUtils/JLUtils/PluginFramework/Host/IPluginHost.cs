namespace JohnLambe.Util.PluginFramework.Host
{
    /// <summary>
    /// Interface to the Plugin Host, as seen by extensions.
    /// </summary>
    public interface IPluginHost
    {
        PluginCollection Plugins { get; }

        /// <summary>
        /// Initialise and add a Plugin.
        /// <para>This fires <see cref="OnPluginInitialize"/>.</para>
        /// </summary>
        /// <param name="plugin">The Plugin instance to be added.</param>
        void AddPlugin(object plugin);

        /// <summary>
        /// Event fired on adding a Plugin to the host, after creation
        /// of the plugin instance.
        /// <para>Conventionally implemented in classes with names ending with "PluginInitializer".</para>
        /// </summary>
        event PluginInitializeDelegate OnPluginInitialize;
    }
}