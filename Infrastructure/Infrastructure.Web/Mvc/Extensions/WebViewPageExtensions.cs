using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
//using Orchard.ContentManagement;
//using Orchard.DisplayManagement;
//using Orchard.DisplayManagement.Shapes;
//using Orchard.Environment.Configuration;
using Infrastructure.Web.Localization;
//using Orchard.Mvc.Html;
//using Orchard.Mvc.Spooling;
//using Infrastructure.Web.Identity;
//using Infrastructure.Web.Identity.Permissions;
//using Orchard.UI.Resources;
using Infrastructure.Web;
using Infrastructure.Core;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.UI.Resources;
using Infrastructure.Web.Mvc.Spooling;
using System.Web.UI;

namespace Infrastructure.Web.Mvc.Extensions
{

    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
          private ScriptRegister _scriptRegister;
          private ResourceRegister _stylesheetRegister;

        private Localizer _localizer = NullLocalizer.Instance;
        private object _display;
        private object _layout;
        private WorkContextBase _workContext;

        public WebViewPage()
        {

        }

        public Localizer T
        {
            get
            {
                // first time used, create it
                if (_localizer == NullLocalizer.Instance)
                {

                    // if the Model is a shape, get localization scopes from binding sources
                    // e.g., Logon.cshtml in a theme, overriging Users/Logon.cshtml, needs T to 
                    // fallback to the one in Users

                    //_workContext = new WebWorkContext((HttpContextBase)Context);



                    // not a shape, use the VirtualPath as scope
                    _localizer = LocalizationUtilities.Resolve(ViewContext, VirtualPath);

                }

                return _localizer;
            }
        }

      //  public dynamic Display { get { return _display; } }
        // review: (heskew) is it going to be a problem?
        //   public new dynamic Layout { get { return _layout; } }
        public WorkContextBase WorkContext
        {
            get
            {
                if (_workContext == null)
                {
                    _workContext = ViewContext.GetWorkContext();
                }

                return _workContext;
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            var context = EngineContext.Current.Resolve<WorkContextBase>();

            HttpContext.Current.Items["_WorkContextKey"] = context;

            _workContext = context;

            //_workContext = ViewContext.GetWorkContext();

            //_display = DisplayHelperFactory.CreateHelper(ViewContext, this);
            _layout = _workContext.Layout;
            //Layout = "";

        }


        public string LanguageCode { get { return WorkContext.WorkingLanguage.Name; } }

        private IResourceManager _resourceManager;
        public IResourceManager ResourceManager
        {
            get { return _resourceManager ?? (_resourceManager = _workContext.Resolve<IResourceManager>()); }
        }

        public ScriptRegister Script
        {
            get
            {
                return _scriptRegister ??
                    (_scriptRegister = new WebViewScriptRegister(this, Html.ViewDataContainer, ResourceManager));
            }
        }


        public ResourceRegister Style
        {
            get
            {
                return _stylesheetRegister ??
                    (_stylesheetRegister = new ResourceRegister(Html.ViewDataContainer, ResourceManager, "stylesheet"));
            }
        }

        public string BaseModuleUrl
        {
            get{
            string  _viewVirtualPath="";
            var templateControl = Html.ViewDataContainer as TemplateControl;
            if (templateControl != null)
            {
                _viewVirtualPath = templateControl.AppRelativeVirtualPath;
            }
            else
            {
                var webPage = Html.ViewDataContainer as WebPageBase;
                if (webPage != null)
                {
                    _viewVirtualPath = webPage.VirtualPath;
                }
            }

           var path= ResourceDefinition.GetBasePathFromViewPath("", _viewVirtualPath);

           return VirtualPathUtility.ToAbsolute(path);
           
            }
        }


        //private IAuthorizer _authorizer;
        //public IAuthorizer Authorizer
        //{
        //    get
        //    {
        //        return _authorizer ?? (_authorizer = _workContext.Resolve<IAuthorizer>());
        //    }
        //}


        //public void RegisterImageSet(string imageSet, string style = "", int size = 16)
        //{
        //    // hack to fake the style "alternate" for now so we don't have to change stylesheet names when this is hooked up
        //    // todo: (heskew) deal in shapes so we have real alternates 
        //    var imageSetStylesheet = !string.IsNullOrWhiteSpace(style)
        //        ? string.Format("{0}-{1}.css", imageSet, style)
        //        : string.Format("{0}.css", imageSet);
        //    Style.Include(imageSetStylesheet);
        //}

        //public virtual void RegisterLink(LinkEntry link)
        //{
        //    ResourceManager.RegisterLink(link);
        //}

        //public void SetMeta(string name = null, string content = null, string httpEquiv = null, string charset = null)
        //{
        //    var metaEntry = new MetaEntry();

        //    if (!String.IsNullOrEmpty(name))
        //    {
        //        metaEntry.Name = name;
        //    }

        //    if (!String.IsNullOrEmpty(content))
        //    {
        //        metaEntry.Content = content;
        //    }

        //    if (!String.IsNullOrEmpty(httpEquiv))
        //    {
        //        metaEntry.HttpEquiv = httpEquiv;
        //    }

        //    if (!String.IsNullOrEmpty(charset))
        //    {
        //        metaEntry.Charset = charset;
        //    }

        //    SetMeta(metaEntry);
        //}

        //public virtual void SetMeta(MetaEntry meta)
        //{
        //    ResourceManager.SetMeta(meta);
        //}

        //public void AppendMeta(string name, string content, string contentSeparator)
        //{
        //    AppendMeta(new MetaEntry { Name = name, Content = content }, contentSeparator);
        //}

        //public virtual void AppendMeta(MetaEntry meta, string contentSeparator)
        //{
        //    ResourceManager.AppendMeta(meta, contentSeparator);
        //}

        //public override void InitHelpers()
        //{
        //    base.InitHelpers();

        //    _workContext = ViewContext.GetWorkContext();

        //    _display = DisplayHelperFactory.CreateHelper(ViewContext, this);
        //    _layout = _workContext.Layout;
        //}

        //public bool AuthorizedFor(Permission permission)
        //{
        //    return Authorizer.Authorize(permission);
        //}

        //public bool HasText(object thing)
        //{
        //    return !string.IsNullOrWhiteSpace(Convert.ToString(thing));
        //}

        //public OrchardTagBuilder Tag(dynamic shape, string tagName)
        //{
        //    return Html.GetWorkContext().Resolve<ITagBuilderFactory>().Create(shape, tagName);
        //}

        //public IHtmlString DisplayChildren(dynamic shape)
        //{
        //    var writer = new HtmlStringWriter();
        //    foreach (var item in shape)
        //    {
        //        writer.Write(Display(item));
        //    }
        //    return writer;
        //}

        //private string _tenantPrefix;
        //public override string Href(string path, params object[] pathParts)
        //{
        //    if (_tenantPrefix == null)
        //    {
        //        _tenantPrefix = WorkContext.Resolve<ShellSettings>().RequestUrlPrefix ?? "";
        //    }

        //    if (!String.IsNullOrEmpty(_tenantPrefix))
        //    {

        //        if (path.StartsWith("~/")
        //            && !path.StartsWith("~/Modules", StringComparison.OrdinalIgnoreCase)
        //            && !path.StartsWith("~/Themes", StringComparison.OrdinalIgnoreCase)
        //            && !path.StartsWith("~/Media", StringComparison.OrdinalIgnoreCase)
        //            && !path.StartsWith("~/Core", StringComparison.OrdinalIgnoreCase))
        //        {

        //            return base.Href("~/" + _tenantPrefix + path.Substring(2), pathParts);
        //        }

        //    }

        //    return base.Href(path, pathParts);
        //}


        public string Display(ResourceLocation Location)
        {
            ResourceManager.WriteResource(Output, Location);
            return "";
        }

        public IDisposable Capture(Action<IHtmlString> callback)
        {
            return new CaptureScope(this, callback);
        }

        public IDisposable Capture(dynamic zone, string position = null)
        {
            return new CaptureScope(this, html => zone.Add(html, position));
        }

        class CaptureScope : IDisposable
        {
            readonly WebPageBase _viewPage;
            readonly Action<IHtmlString> _callback;

            public CaptureScope(WebPageBase viewPage, Action<IHtmlString> callback)
            {
                _viewPage = viewPage;
                _callback = callback;
                _viewPage.OutputStack.Push(new HtmlStringWriter());
            }

            void IDisposable.Dispose()
            {
                var writer = (HtmlStringWriter)_viewPage.OutputStack.Pop();
                _callback(writer);
            }
        }

        class WebViewScriptRegister : ScriptRegister
        {
            private readonly WebPageBase _viewPage;

            public WebViewScriptRegister(WebPageBase viewPage, IViewDataContainer container, IResourceManager resourceManager)
                : base(container, resourceManager)
            {
                _viewPage = viewPage;
            }

            public override IDisposable Head()
            {
                return new CaptureScope(_viewPage, s => ResourceManager.RegisterHeadScript(s.ToString()));
            }

            public override IDisposable Foot()
            {
                return new CaptureScope(_viewPage, s => ResourceManager.RegisterFootScript(s.ToString()));
            }
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}
