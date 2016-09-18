namespace JohnLambe.Util.PluginFramework.Host
{
    public class PluginInitializerBase
    {
        public PluginInitializerBase(IPluginHost host)
        {
            host.OnPluginInitialize += PluginInitialize;
        }

        protected virtual void PluginInitialize(PluginInitializeParams parameters)
        {
        }
    }
}