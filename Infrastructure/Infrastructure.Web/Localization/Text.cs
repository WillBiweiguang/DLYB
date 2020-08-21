using System;
using System.Globalization;
using System.Linq;
using System.Web;
using Infrastructure.Web.Localization.Services;
using Infrastructure.Web.Logging;
using Infrastructure.Core.Logging;
using Infrastructure.Core;

namespace Infrastructure.Web.Localization
{
    public class Text : IText {
        private readonly string _scope;
       // private readonly IWorkContextAccessor _workContextAccessor;
        private readonly WorkContextBase _workContext;
        private readonly ILocalizedStringManager _localizedStringManager;

        public Text(string scope, WorkContextBase workContext, /*IWorkContextAccessor workContextAccessor,*/ ILocalizedStringManager localizedStringManager)
        {
            _scope = scope;
           // _workContextAccessor = workContextAccessor;
            _workContext = workContext;
            _localizedStringManager = localizedStringManager;
            Logger = LogManager.GetLogger(this.GetType()); //NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public LocalizedString Get(string textHint, params object[] args) {
           // Logger.Debug("{0} localizing '{1}'", _scope, textHint);

           // return new LocalizedString(textHint, _scope, textHint, args);

            var workContext = _workContext.WorkingLanguage;// _workContextAccessor.GetContext();
	        
	        if (workContext != null) {
		        var currentCulture = workContext.Name;
		        var localizedFormat = _localizedStringManager.GetLocalizedString(_scope, textHint, currentCulture);

				return args.Length == 0
				? new LocalizedString(localizedFormat, _scope, textHint, args)
				: new LocalizedString(string.Format(GetFormatProvider(currentCulture), localizedFormat, args), _scope, textHint, args);
	        }

			return new LocalizedString(textHint, _scope, textHint, args);
        }

        private static IFormatProvider GetFormatProvider(string currentCulture) {
            try {
                return CultureInfo.GetCultureInfoByIetfLanguageTag(currentCulture);
            }
            catch {
                return null;
            }
        }
    }
}