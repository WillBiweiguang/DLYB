using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
//using Infrastructure.Core.ComponentModel;
using Infrastructure.Core.Plugins;
using Infrastructure.Core.ComponentModel;
using Infrastructure.Core.Logging;

//Contributor: Umbraco (http://www.umbraco.com). Thanks a lot! 
//SEE THIS POST for full details of what this does - http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

//[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]
namespace Infrastructure.Core.Plugins
{
    /// <summary>
    /// Sets the application up for the plugin referencing
    /// </summary>
    public class PluginManager
    {
        #region Const

        private const string InstalledPluginsFilePath = "~/App_Data/InstalledPlugins.txt";
        private const string PluginsPath = "~/Plugins";
        private const string ShadowCopyPath = "~/Plugins/bin";
        private const string PluginConfigName = "Description.txt";

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static DirectoryInfo _shadowCopyFolder;
        private static bool _clearShadowDirectoryOnStartup;

        private static ILogger LoggerMan = LogManager.GetLogger(typeof(PluginManager));
        //  private static ILogger LoggerMan = LogManager.GetLogger("WeChat");
        #endregion

        #region Methods


        public static IEnumerable<PluginDescriptor> AllPlugins { get; set; }

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get { return AllPlugins.ToList().FindAll(a => a.Installed); } }


        public static IEnumerable<PluginDescriptor> UnInstallPlugins { get { return AllPlugins.ToList().FindAll(a => !a.Installed); } }

        /// <summary>
        /// Returns a collection of all plugin which are not compatible with the current version
        /// </summary>
        public static IEnumerable<string> IncompatiblePlugins { get; set; }

        private static Dictionary<string, string> InstalledPluginsDll { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
            using (new WriteLockDisposable(Locker))
            {
                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether
                var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(PluginsPath));
                _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(ShadowCopyPath));

                //var referencedPlugins = new List<PluginDescriptor>();
                var allPlugins = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();

                InstalledPluginsDll = new Dictionary<string, string>();

                _clearShadowDirectoryOnStartup = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]) &&
                   Convert.ToBoolean(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]);

                try
                {
                    //  var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());

                    // Debug.WriteLine("Creating shadow copy folder and querying for dlls");
                    LoggerMan.Debug("Creating shadow copy folder and querying for dlls");

                    //ensure folders are created
                    Directory.CreateDirectory(pluginFolder.FullName);
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

                    //get list of all files in bin
                    var binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                    if (_clearShadowDirectoryOnStartup)
                    {
                        //clear out shadow copied plugins
                        foreach (var f in binFiles)
                        {
                            Debug.WriteLine("Deleting " + f.Name);
                            try
                            {
                                File.Delete(f.FullName);
                            }
                            catch (Exception exc)
                            {
                                // Debug.WriteLine("Error deleting file " + f.Name + ". Exception: " + exc);
                                LoggerMan.Error(exc);
                            }
                        }
                    }

                    var allPluginsDescriptors = GetDescriptionFilesAndDescriptors(pluginFolder);

                    //load description files
                    foreach (var dfd in allPluginsDescriptors)
                    {
                        try
                        {

                            var descriptionFile = dfd.Key;
                            var pluginDescriptor = dfd.Value;

                            if (String.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
                                throw new Exception(string.Format("A plugin '{0}' has no system name. Try assigning the plugin a unique name and recompiling.", descriptionFile.FullName));
                            if (allPlugins.Contains(pluginDescriptor))
                                throw new Exception(string.Format("A plugin with '{0}' system name is already defined", pluginDescriptor.SystemName));
                            LoggerMan.Debug(string.Format("Installing {0}...", pluginDescriptor.SystemName));

                            //预安装。当install标记是-1时
                            if (!pluginDescriptor.NeedInstalled && !string.IsNullOrEmpty(pluginDescriptor.InstallFrom))
                            {
                                string strModule = string.Format("{0}\\{1}", pluginFolder, pluginDescriptor.SystemName);

                                var basePath = HostingEnvironment.MapPath(PluginsPath);

                                //文件备份到Backup目录
                                string strTemp = basePath + "BackUp\\" + pluginDescriptor.SystemName + DateTime.Now.ToString("yyyyMMddHHmmss");

                                try
                                {
                                    if (!System.IO.Directory.Exists(basePath + "BackUp\\"))
                                    {
                                        System.IO.Directory.CreateDirectory(basePath + "BackUp\\");
                                    }


                                    System.IO.Directory.Move(strModule, strTemp);
                                    //System.IO.Directory.Delete(str, true);
                                }
                                catch (Exception ex)
                                {
                                    LoggerMan.Error(ex, "Delete Folder Error!");
                                    System.IO.Directory.Move(strModule, strTemp);

                                }

                                //拷贝临时文件夹中的文件到module目录

                                // 4.1.20160909.02
                                // 4 - 大版本号
                                // 1 - 小版本号
                                // 20160909 - 不重要，自定义
                                // 02 - 发布号，每次加1
                                var pluginsVersions = pluginDescriptor.Version.Split('.');

                                var majorVersion = pluginsVersions[0];
                                var minerVersion = pluginsVersions[1];
                                var defineVersion = pluginsVersions[2];

                                // 如果缺位，默认缺发布号
                                int releaseNo = (pluginsVersions.Length == 4 ? int.Parse(pluginsVersions[3]) : 0);
                                releaseNo++;

                                var newVersion = string.Format("{0}.{1}.{2}.{3}", majorVersion, minerVersion, defineVersion, releaseNo);
                                // string strPathTo = string.Format("{0}Plugins\\{1}", basePath, systemName);
                                System.IO.Directory.Move(Path.Combine(HostingEnvironment.MapPath("~/pluginstemp"), pluginDescriptor.InstallFrom), strModule);
                                //var plug = PluginManager.GetPluginDescriptor(Directory.CreateDirectory(strModule));
                                pluginDescriptor.Version = newVersion;
                                pluginDescriptor.InstallFrom = "";
                                PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
                                //PluginManager.SavePlugins(plug);

                            }


                            //没有安装plugin的不应该加载（影响效率），但是如果不加载，那么在安装的时候无法执行plugin的 Install方法
                            if (pluginDescriptor.Installed)
                            {
                                SetPluginInstalled(descriptionFile, pluginDescriptor, binFiles, allPluginsDescriptors);
                            }

                            allPlugins.Add(pluginDescriptor);
                            // pluginDescriptor.Instance().Initialize();
                        }
                        catch (Exception ex)
                        {
                            LoggerMan.Error(ex);
                        }

                    }


                    AllPlugins = allPlugins;
                    IncompatiblePlugins = incompatiblePlugins;

                    var referencedPlugins = ReferencedPlugins;

                    foreach (var a in referencedPlugins)
                    {
                        if (a.IsEnabled)
                        {
                            a.Instance().Initialize();
                            if (a.NeedInstalled)
                            {
                                a.Instance().Install();
                                a.NeedInstalled = false;
                                InstallPlugin(a);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //var msg = string.Empty;
                    //for (var e = ex; e != null; e = e.InnerException)
                    //    msg += e.Message + Environment.NewLine;

                    //var fail = new Exception(msg, ex);
                    //Debug.WriteLine(fail.Message, fail);

                    //  LoggerMan.
                    LoggerMan.Error(ex);
                    //throw ex;
                }


                // ReferencedPlugins = referencedPlugins;
                // UnInstallPlugins = uninstallPlugins;



            }
        }

        /// <summary>
        /// SavePlugins
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        public static void SavePlugins(
         PluginDescriptor pluginDescriptor)
        {
            AllPlugins.ToList().Add(pluginDescriptor);
        }


        /// <summary>
        /// SetPluginInstalled
        /// </summary>
        /// <param name="descriptionFile"></param>
        /// <param name="pluginDescriptor"></param>
        /// <param name="binFiles"></param>
        public static bool SetPluginInstalled(FileInfo descriptionFile,
            PluginDescriptor pluginDescriptor, FileInfo[] binFiles, IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> allPluginsDescriptors)
        {
            if (pluginDescriptor.IsEnabled)
            {
                LoggerMan.Debug(string.Format("Plugin {0} is already installed, skipping...", pluginDescriptor.SystemName));
                return true;
            }

            if (!pluginDescriptor.Installed)
            {
                LoggerMan.Debug(string.Format("Plugin {0} is not planned to install, skipping...", pluginDescriptor.SystemName));
                return false;
            }

            // 安装dependences
            foreach (var p in pluginDescriptor.Dependences)
            {
                var dependencePlugin = allPluginsDescriptors.FirstOrDefault(x => x.Value.SystemName == p);

                LoggerMan.Debug(string.Format("Installing Dependence plugin {0} - 1 ...", dependencePlugin.Value.SystemName));

                if (dependencePlugin.Equals(default(IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>>)))
                {
                    // 找不到自己dependence的情况。。。
                    dependencePlugin.Value.RuntimeInformation = string.Format("Plugin '{0}' start up failed, Dependence {1} can't be found.", dependencePlugin.Value.SystemName, p);
                    LoggerMan.Error(dependencePlugin.Value.RuntimeInformation);
                    break;
                }

                LoggerMan.Debug(string.Format("Installing Dependence plugin {0} - 2 ...", dependencePlugin.Value.SystemName));
                bool ret = SetPluginInstalled(dependencePlugin.Key, dependencePlugin.Value, binFiles, allPluginsDescriptors);
                LoggerMan.Debug(string.Format("Installed Dependence plugin {0}", dependencePlugin.Value.SystemName));
                if (!ret)
                {
                    return false;
                }
            }

            try
            {
                if (binFiles == null)
                {
                    binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                }
                if (descriptionFile.Directory == null)
                    throw new Exception(string.Format("Directory cannot be resolved for '{0}' description file", descriptionFile.Name));
                //get list of all DLLs in plugins (not in bin!)
                var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                    //just make sure we're not registering shadow copied plugins
                    .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                    .Where(x => IsPackagePluginFolder(x.Directory))
                    .ToList();

                //other plugin description info
                var mainPluginFile = pluginFiles
                    .FirstOrDefault(x => x.Name.Equals(pluginDescriptor.PluginFileName, StringComparison.InvariantCultureIgnoreCase));
                pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

                if (mainPluginFile == null)
                {
                    LoggerMan.Error("{0} main dll not found!", pluginDescriptor.SystemName);
                    return false;
                }

                Assembly objLoaded;
                if (!IsAlreadyLoaded(mainPluginFile,out objLoaded))
                {
                    //shadow copy main plugin file
                    var mainPluginFilename = PerformFileDeploy(mainPluginFile);

                    pluginDescriptor.ReferencedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(mainPluginFilename));
                    BuildManager.AddReferencedAssembly(pluginDescriptor.ReferencedAssembly);
                }
                else
                {
                    pluginDescriptor.ReferencedAssembly = objLoaded;
                    BuildManager.AddReferencedAssembly(pluginDescriptor.ReferencedAssembly);
                }


                //load all other referenced assemblies now
                var pluginAssembliyNames = new List<String>();
                foreach (var plugin in pluginFiles
                    .Where(x => !x.Name.Equals(mainPluginFile.Name, StringComparison.InvariantCultureIgnoreCase))
                    .Where(x => !IsAlreadyLoaded(x,out objLoaded)))
                {
                    pluginAssembliyNames.Add(PerformFileDeploy(plugin));

                    //标记哪些dll已经拷贝
                    InstalledPluginsDll.Add(plugin.Name, plugin.FullName);
                }


                foreach (var assemblyName in pluginAssembliyNames)
                {
                    //we can now register the plugin definition
                    var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyName));

                    LoggerMan.Debug("Adding to BuildManager: '{0}'", shadowCopiedAssembly.FullName);
                    BuildManager.AddReferencedAssembly(shadowCopiedAssembly);
                }

               

                //init plugin type (only one plugin per assembly is allowed)
                foreach (var t in pluginDescriptor.ReferencedAssembly.GetTypes())
                    if (typeof(IPlugin).IsAssignableFrom(t))
                        if (!t.IsInterface)
                            if (t.IsClass && !t.IsAbstract)
                            {
                                pluginDescriptor.PluginType = t;
                                break;
                            }

                pluginDescriptor.IsEnabled = true;
                LoggerMan.Debug(string.Format("Installed {0}...", pluginDescriptor.SystemName));
                return true;
            }
            catch (ReflectionTypeLoadException ex)
            {
                //var msg = string.Empty;
                //foreach (var e in ex.LoaderExceptions)
                //    msg += e.Message + Environment.NewLine;

                //var fail = new Exception(msg, ex);
                //Debug.WriteLine(fail.Message, fail);

                LoggerMan.Error(ex);
                LoggerMan.Error(ex.LoaderExceptions);

                throw ex;
            }
        }
        /// <summary>
        /// ExcuteAllInstallEvent
        /// </summary>
        public static void ExcuteAllInstallEvent()
        {
            var pls = AllPlugins.ToList().FindAll(a => a.Installed && a.NeedInstalled);
            foreach (var a in pls)
            {
                // 如果已安装过则跳过，未安装过的往下走，如果未安装的发现自己有Dependence则先安装Dependence，深度优先安装
                if (a.IsEnabled)
                {
                    continue;
                }
                a.NeedInstalled = false;
                a.IsEnabled = false;
                a.Installed = true;

                // 安装dependences
                foreach (var p in a.Dependences)
                {
                    var dp = pls.FirstOrDefault(x => x.SystemName == p);
                    if (dp == null)
                    {
                        // 找不到自己dependence的情况。。。
                        a.RuntimeInformation = string.Format("Plugin '{0}' start up failed, Dependence {1} can't be found.", a.SystemName, p);
                        LoggerMan.Error(a.RuntimeInformation);
                        break;
                    }

                    InstallPlugin(dp);
                }

                // 安装自己
                InstallPlugin(a);
            }
        }

        private static void InstallPlugin(PluginDescriptor a)
        {
            PluginFileParser.SavePluginDescriptionFile(a);

            try
            {
                a.Instance().Install();
                a.IsEnabled = true;
            }
            catch (Exception ex)
            {
                a.NeedInstalled = true;
                a.Installed = false;
                PluginFileParser.SavePluginDescriptionFile(a);
                LoggerMan.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        public static void MarkPluginInstallEvent(PluginDescriptor pluginDescriptor)
        {
            //if (String.IsNullOrEmpty(systemName))
            //throw new ArgumentNullException("systemName");
            //var pl = AllPlugins.ToList().Find(a => a.SystemName == systemName);
            try
            {
                pluginDescriptor.Installed = true;
                pluginDescriptor.NeedInstalled = true;
                PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
            }
            catch (Exception ex)
            {
                pluginDescriptor.Installed = false;
                pluginDescriptor.NeedInstalled = false;
                LoggerMan.Error(ex);
            }

        }

        private static void MarkPlugin(string systemName, bool bolIn)
        {
            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");
            var pl = AllPlugins.ToList().Find(a => a.SystemName == systemName);
            if (pl.Installed != bolIn)
            {
                pl.Installed = bolIn;
                pl.NeedInstalled = bolIn;
                try
                {
                    PluginFileParser.SavePluginDescriptionFile(pl);
                }
                catch (Exception ex)
                {
                    pl.Installed = !bolIn;
                    LoggerMan.Error(ex);
                }

            }
        }

        /// <summary>
        /// Mark plugin as installed
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsInstalled(string systemName)
        {

            MarkPlugin(systemName, true);


            //if (String.IsNullOrEmpty(systemName))
            //    throw new ArgumentNullException("systemName");

            //var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            //if (!File.Exists(filePath))
            //    using (File.Create(filePath))
            //    {
            //        //we use 'using' to close the file after it's created
            //    }


            //var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
            //bool alreadyMarkedAsInstalled = installedPluginSystemNames
            //                    .FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            //if (!alreadyMarkedAsInstalled)
            //    installedPluginSystemNames.Add(systemName);
            //PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsUninstalled(string systemName)
        {

            MarkPlugin(systemName, false);
            //if (String.IsNullOrEmpty(systemName))
            //    throw new ArgumentNullException("systemName");

            //var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            //if (!File.Exists(filePath))
            //    using (File.Create(filePath))
            //    {
            //        //we use 'using' to close the file after it's created
            //    }


            //var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
            //bool alreadyMarkedAsInstalled = installedPluginSystemNames
            //                    .FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            //if (alreadyMarkedAsInstalled)
            //    installedPluginSystemNames.Remove(systemName);
            //PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        public static void MarkAllPluginsAsUninstalled()
        {

            foreach (var a in AllPlugins)
            {
                MarkPlugin(a.SystemName, false);
            }

            //var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            //if (File.Exists(filePath))
            //    File.Delete(filePath);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get description files
        /// </summary>
        /// <param name="pluginFolder">Plugin direcotry info</param>
        /// <returns>Original and parsed description files</returns>
        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(DirectoryInfo pluginFolder)
        {
            if (pluginFolder == null)
                throw new ArgumentNullException("pluginFolder");

            //create list (<file info, parsed plugin descritor>)
            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();
            //add display order and path to list
            foreach (var descriptionFile in pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories))
            {
                if (!IsPackagePluginFolder(descriptionFile.Directory))
                    continue;

                //parse file
                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);

                pluginDescriptor.PluginPath = System.IO.Path.GetDirectoryName(descriptionFile.FullName);

                //populate list
                result.Add(new KeyValuePair<FileInfo, PluginDescriptor>(descriptionFile, pluginDescriptor));
            }

            //sort list by display order. NOTE: Lowest DisplayOrder will be first i.e 0 , 1, 1, 1, 5, 10
            //it's required: http://www.nopcommerce.com/boards/t/17455/load-plugins-based-on-their-displayorder-on-startup.aspx
            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
            return result;
        }

        public static PluginDescriptor GetPluginDescriptor(DirectoryInfo pluginFolder)
        {
            //pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectorsties);

            var Files = pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories);

            if (Files.Length > 0)
            {
                var descriptionFile = Files[0];
                if (!IsPackagePluginFolder(descriptionFile.Directory))
                    return null;

                //parse file
                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);

                pluginDescriptor.PluginPath = System.IO.Path.GetDirectoryName(descriptionFile.FullName);

                return pluginDescriptor;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Indicates whether assembly file is already loaded
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <returns>Result</returns>
        private static bool IsAlreadyLoaded(FileInfo fileInfo, out Assembly objLoaded)
        {
            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.FullName, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (fileNameWithoutExt == null)
                    throw new Exception(string.Format("Cannot get file extnension for {0}", fileInfo.Name));
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string assemblyName = a.FullName.Split(new[] { ',' }).FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        objLoaded = a;
                        return true;
                    }

                }

                if (InstalledPluginsDll.Keys.Contains(fileInfo.Name))
                {
                    objLoaded = null;
                    return true;
                }
            }
            catch (Exception exc)
            {
                // Debug.WriteLine("Cannot validate whether an assembly is already loaded. " + exc);
                LoggerMan.Error(exc);
            }

            objLoaded = null;
            return false;
        }

        /// <summary>
        /// Perform file deply
        /// </summary>
        /// <param name="plug">Plugin file info</param>
        /// <returns>Assembly</returns>
        private static string PerformFileDeploy(FileInfo plug)
        {
            if (plug.Directory.Parent == null)
                throw new InvalidOperationException("The plugin directory for the " + plug.Name +
                                                    " file exists in a folder outside of the allowed nopCommerce folder heirarchy");

            FileInfo shadowCopiedPlug;

            if (CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            {
                //all plugins will need to be copied to ~/Plugins/bin/
                //this is aboslutely required because all of this relies on probingPaths being set statically in the web.config

                //were running in med trust, so copy to custom bin folder
                var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
                shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);
            }
            else
            {
                var directory = AppDomain.CurrentDomain.DynamicDirectory;
                // Debug.WriteLine(plug.FullName + " to " + directory);

                LoggerMan.Debug(plug.FullName + " to " + directory);
                //were running in full trust so copy to standard dynamic folder
                shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(directory));
            }

            return shadowCopiedPlug.FullName;
        }

        /// <summary>
        /// Used to initialize plugins when running in Full Trust
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeFullTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException ex)
            {
                // Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");

                LoggerMan.Error(ex, shadowCopiedPlug.FullName + " is locked, attempting to rename");

                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                //ok, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }

        /// <summary>
        /// Used to initialize plugins when running in Medium Trust
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeMediumTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    // Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);

                    LoggerMan.Debug("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);

                    shouldCopy = false;
                }
                else
                {
                    //delete an existing file

                    //More info: http://www.nopcommerce.com/boards/t/11511/access-error-nopplugindiscountrulesbillingcountrydll.aspx?p=4#60838
                    //Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);

                    LoggerMan.Debug("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);

                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (shouldCopy)
            {
                try
                {
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
                catch (IOException)
                {
                    // Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");

                    LoggerMan.Debug(shadowCopiedPlug.FullName + " is locked, attempting to rename");

                    //this occurs when the files are locked,
                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                    try
                    {
                        var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlug.FullName, oldFile);
                    }
                    catch (IOException exc)
                    {
                        throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                    }
                    //ok, we've made it this far, now retry the shadow copy
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
            }

            return shadowCopiedPlug;
        }

        /// <summary>
        /// Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder == null) return false;
            if (folder.Parent == null) return false;
            if (!folder.Parent.Name.Equals("Plugins", StringComparison.InvariantCultureIgnoreCase) && !folder.Parent.Parent.Name.Equals("Plugins", StringComparison.InvariantCultureIgnoreCase)) return false;
            return true;
        }

        /// <summary>
        /// Gets the full path of InstalledPlugins.txt file
        /// </summary>
        /// <returns></returns>
        private static string GetInstalledPluginsFilePath()
        {
            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            return filePath;
        }

        #endregion
    }
}
