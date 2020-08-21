namespace Infrastructure.Core.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// Gets or sets the plugin descriptor
        /// </summary>
        public virtual PluginDescriptor PluginDescriptor { get; set; }


        public virtual void Initialize()
        {
            //PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public virtual void Install() 
        {
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public virtual void Uninstall() 
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
        }

    }
}
