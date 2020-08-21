using System;
//using Infrastructure.Core.Domain.Customers;
//using Infrastructure.Core.Domain.Directory;
//using Infrastructure.Core.Domain.Localization;
//using Infrastructure.Core.Domain.Tax;
//using Infrastructure.Core.Domain.Vendors;
using System.Globalization;

namespace Infrastructure.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        ///// <summary>
        ///// Gets or sets the current customer
        ///// </summary>
        //Customer CurrentCustomer { get; set; }
        ///// <summary>
        ///// Gets or sets the original customer (in case the current one is impersonated)
        ///// </summary>
        //Customer OriginalCustomerIfImpersonated { get; }
        ///// <summary>
        ///// Gets or sets the current vendor (logged-in manager)
        ///// </summary>
        //Vendor CurrentVendor { get; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        CultureInfo WorkingLanguage { get; set; }

        TimeZoneInfo CurrentTimeZone { get; set; }
        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        //Currency WorkingCurrency { get; set; }
        ///// <summary>
        ///// Get or set current tax display type
        ///// </summary>
        //TaxDisplayType TaxDisplayType { get; set; }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
