namespace PublicHolidays.API.Contracts
{
    public class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;
        
        
        public static class Holidays
        {
            public const string GetAllForActualYear = Base + "/" + "holidays";

            public const string GetHolidaysFromDateToDate = Base + "/" + "holidays/filter";
        }
    }
}