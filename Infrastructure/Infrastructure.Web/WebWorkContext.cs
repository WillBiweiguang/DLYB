using System;
using System.Linq;
using System.Web;

using System.Globalization;
using Infrastructure.Core;
using Autofac;
using Microsoft.AspNet.Identity;

namespace Infrastructure.Web
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : WorkContextBase
    {
        #region Const

        private const string CustomerCookieName = "Nop.customer";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        //private readonly ICustomerService _customerService;
        //private readonly IVendorService _vendorService;
        //private readonly IStoreContext _storeContext;
        //private readonly IAuthenticationService _authenticationService;
        //private readonly ILanguageService _languageService;
        //private readonly ICurrencyService _currencyService;
        //private readonly IGenericAttributeService _genericAttributeService;
        //private readonly TaxSettings _taxSettings;
        //private readonly CurrencySettings _currencySettings;
        //private readonly LocalizationSettings _localizationSettings;
        //private readonly IUserAgentHelper _userAgentHelper;
        //private readonly IStoreMappingService _storeMappingService;

        //private Customer _cachedCustomer;
        //private Customer _originalCustomerIfImpersonated;
        //private Vendor _cachedVendor;
        private CultureInfo _cachedLanguage;
        //private Currency _cachedCurrency;
        //private TaxDisplayType? _cachedTaxDisplayType;

        #endregion

        #region Ctor

        readonly IComponentContext _componentContext;

        /// <summary>
        /// WebWorkContext
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="componentContext"></param>
        public WebWorkContext(HttpContextBase httpContext,IComponentContext componentContext
            //ICustomerService customerService,
            //IVendorService vendorService,
            //IStoreContext storeContext,
            //IAuthenticationService authenticationService,
            //ILanguageService languageService,
            //ICurrencyService currencyService,
            //IGenericAttributeService genericAttributeService,
            //TaxSettings taxSettings, 
            //CurrencySettings currencySettings,
            //LocalizationSettings localizationSettings,
            //IUserAgentHelper userAgentHelper,
            //IStoreMappingService storeMappingService
            )
        {
            this._httpContext = httpContext;
             _componentContext = componentContext;
           // _workContextStateProviders = componentContext.Resolve<IEnumerable<IWorkContextStateProvider>>();
            //this._customerService = customerService;
            //this._vendorService = vendorService;
            //this._storeContext = storeContext;
            //this._authenticationService = authenticationService;
            //this._languageService = languageService;
            //this._currencyService = currencyService;
            //this._genericAttributeService = genericAttributeService;
            //this._taxSettings = taxSettings;
            //this._currencySettings = currencySettings;
            //this._localizationSettings = localizationSettings;
            //this._userAgentHelper = userAgentHelper;
            //this._storeMappingService = storeMappingService;
        }

        #endregion

        #region Utilities

        public override T Resolve<T>()
        {
            return _componentContext.Resolve<T>();
        }

        public override bool TryResolve<T>(out T service)
        {
            return _componentContext.TryResolve(out service);
        }


        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[CustomerCookieName];
        }

        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(CustomerCookieName);
                cookie.HttpOnly = true;
                cookie.Value = customerGuid.ToString();
                if (customerGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24*365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(CustomerCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        protected virtual CultureInfo GetLanguageFromUrl()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            string virtualPath = _httpContext.Request.AppRelativeCurrentExecutionFilePath;
            string applicationPath = _httpContext.Request.ApplicationPath;
            //if (!virtualPath.IsLocalizedUrl(applicationPath, false))
            //    return null;

            //var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
            //if (String.IsNullOrEmpty(seoCode))
            //    return null;

            //var language = _languageService
            //    .GetAllLanguages()
            //    .FirstOrDefault(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            //if (language != null && language.Published && _storeMappingService.Authorize(language))
            //{
            //    return language;
            //}

            return null;
        }

        protected virtual CultureInfo GetLanguageFromBrowserSettings()
        {
            if (_httpContext == null ||
                _httpContext.Request == null ||
                _httpContext.Request.UserLanguages == null)
                return null;

            var userLanguage = _httpContext.Request.UserLanguages.FirstOrDefault();
            if (String.IsNullOrEmpty(userLanguage))
                return null;

            return new System.Globalization.CultureInfo(userLanguage.Substring(0,2));

            //var language = _languageService
            //    .GetAllLanguages()
            //    .FirstOrDefault(l => userLanguage.Equals(l.LanguageCulture, StringComparison.InvariantCultureIgnoreCase));
            //if (language != null && language.Published && _storeMappingService.Authorize(language))
            //{
            //    return language;
            //}

           // return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public override CultureInfo WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                CultureInfo detectedLanguage = null;
                //if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                //{
                //    //get language from URL
                //    detectedLanguage = GetLanguageFromUrl();
                //}
                //if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                //{
                //    //get language from browser settings
                //    //but we do it only once
                //    if (!this.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.LanguageAutomaticallyDetected, 
                //        _genericAttributeService, _storeContext.CurrentStore.Id))
                //    {
                //        detectedLanguage = GetLanguageFromBrowserSettings();
                //        if (detectedLanguage != null)
                //        {
                //            _genericAttributeService.SaveAttribute(this.CurrentCustomer, SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
                //                 true, _storeContext.CurrentStore.Id);
                //        }
                //    }
                //}

                var objUserInfo=_httpContext.Items["UserInfo"] as EntityUser;

                if (objUserInfo != null)
                {
                    detectedLanguage = objUserInfo.objCulture;
                }

                if (detectedLanguage == null)
                {
                    //it not specified, then return the first (filtered by current store) found one
                    detectedLanguage =GetLanguageFromBrowserSettings();
                }

                //cache
                _cachedLanguage = detectedLanguage;
                return _cachedLanguage;
            }
            set
            {


                //reset cache
                _cachedLanguage = null;
            }
        }

        ///// <summary>
        ///// Get or set current user working currency
        ///// </summary>
        //public virtual CultureInfo WorkingCurrency
        //{
        //    get
        //    {
        //        if (_cachedCurrency != null)
        //            return _cachedCurrency;
                
        //        //return primary store currency when we're in admin area/mode
        //        if (this.IsAdmin)
        //        {
        //            var primaryStoreCurrency =  _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
        //            if (primaryStoreCurrency != null)
        //            {
        //                //cache
        //                _cachedCurrency = primaryStoreCurrency;
        //                return primaryStoreCurrency;
        //            }
        //        }

        //        var allCurrencies = _currencyService.GetAllCurrencies(storeId: _storeContext.CurrentStore.Id);
        //        //find a currency previously selected by a customer
        //        var currencyId = this.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId,
        //            _genericAttributeService, _storeContext.CurrentStore.Id);
        //        var currency = allCurrencies.FirstOrDefault(x => x.Id == currencyId);
        //        if (currency == null)
        //        {
        //            //it not found, then let's load the default currency for the current language (if specified)
        //            currencyId = this.WorkingLanguage.DefaultCurrencyId;
        //            currency = allCurrencies.FirstOrDefault(x => x.Id == currencyId);
        //        }
        //        if (currency == null)
        //        {
        //            //it not found, then return the first (filtered by current store) found one
        //            currency = allCurrencies.FirstOrDefault();
        //        }
        //        if (currency == null)
        //        {
        //            //it not specified, then return the first found one
        //            currency = _currencyService.GetAllCurrencies().FirstOrDefault();
        //        }

        //        //cache
        //        _cachedCurrency = currency;
        //        return _cachedCurrency;
        //    }
        //    set
        //    {
        //        var currencyId = value != null ? value.Id : 0;
        //        _genericAttributeService.SaveAttribute(this.CurrentCustomer,
        //            SystemCustomerAttributeNames.CurrencyId,
        //            currencyId, _storeContext.CurrentStore.Id);

        //        //reset cache
        //        _cachedCurrency = null;
        //    }
        //}

        ///// <summary>
        ///// Get or set current tax display type
        ///// </summary>
        //public virtual TaxDisplayType TaxDisplayType
        //{
        //    get
        //    {
        //        //cache
        //        if (_cachedTaxDisplayType != null)
        //            return _cachedTaxDisplayType.Value;

        //        TaxDisplayType taxDisplayType;
        //        if (_taxSettings.AllowCustomersToSelectTaxDisplayType && this.CurrentCustomer != null)
        //        {
        //            taxDisplayType = (TaxDisplayType) this.CurrentCustomer.GetAttribute<int>(
        //                SystemCustomerAttributeNames.TaxDisplayTypeId,
        //                _genericAttributeService,
        //                _storeContext.CurrentStore.Id);
        //        }
        //        else
        //        {
        //            taxDisplayType = _taxSettings.TaxDisplayType;
        //        }

        //        //cache
        //        _cachedTaxDisplayType = taxDisplayType;
        //        return _cachedTaxDisplayType.Value;

        //    }
        //    set
        //    {
        //        if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
        //            return;

        //        _genericAttributeService.SaveAttribute(this.CurrentCustomer, 
        //            SystemCustomerAttributeNames.TaxDisplayTypeId,
        //            (int)value, _storeContext.CurrentStore.Id);

        //        //reset cache
        //        _cachedTaxDisplayType = null;

        //    }
        //}

        public IUser<int> CurrentUser
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}
