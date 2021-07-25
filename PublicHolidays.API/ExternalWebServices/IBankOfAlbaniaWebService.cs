using System.Collections.Generic;
using System.Threading.Tasks;
using PublicHolidays.API.Domain;

namespace PublicHolidays.API.ExternalWebServices
{
    public interface IBankOfAlbaniaWebService
    {
        Task<int> GetYearOfHolidaysAsync();
        
        Task<IEnumerable<Holiday>> GetListOfHolidaysAsync();
    }
}