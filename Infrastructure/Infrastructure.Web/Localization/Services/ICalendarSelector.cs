using System.Web;

namespace Infrastructure.Utility.Localization.Services
{
	public class CalendarSelectorResult {
		public int Priority {
			get;
			set;
		}
		public string CalendarName {
			get;
			set;
		}
	}

	public interface ICalendarSelector : IDependency {
		CalendarSelectorResult GetCalendar(HttpContextBase context);
	}
}
