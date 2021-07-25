using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicHolidays.API.Contracts;
using PublicHolidays.API.Contracts.Responses;
using PublicHolidays.API.Domain;
using PublicHolidays.API.ExternalWebServices;

namespace PublicHolidays.API.Controllers.V1
{

    [ApiController]
    [Produces("application/json")]
    public class HolidaysController : ControllerBase
    {
        private readonly IBankOfAlbaniaWebService _service;

        public HolidaysController(IBankOfAlbaniaWebService service)
        {
            _service = service;
        }

        
        /// <summary>
        ///     Kthen listen e pushimeve zyrtare per gjithe vitin
        /// </summary>
        ///
        /// <returns>Listen me te dhenat e diteve te pushimit</returns>>
        /// <response code="200"> Kthen listen me te dhenat </response>
        /// <response code="500"> Kthen nje mesazh me gabimin qe ka ndodhur gjate marrjes se te dhenave  </response>

        [HttpGet(ApiRoutes.Holidays.GetAllForActualYear)]
        [ProducesResponseType(typeof(List<Holiday>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessageResponse),StatusCodes.Status500InternalServerError)]
        public async Task <IActionResult> GetHolidaysForActualYear()
        {
            try
            {
                var result = await _service.GetListOfHolidaysAsync();

                return Ok(result);
            }
            catch (AggregateException aggregateException)
            {
                var builder = new StringBuilder();

                // catch whatever was thrown
                foreach (Exception ex in aggregateException.InnerExceptions)
                {
                    builder.AppendLine($"Type: { aggregateException.GetType() },  Message: { aggregateException.Message } {Environment.NewLine}");
                }
                
                return StatusCode(StatusCodes.Status500InternalServerError, builder.ToString()); 
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
        
        /// <summary>
        ///     Kthen listen e pushimeve zyrtare per nje interval date, date fillimi deri date mbarimi
        /// </summary>
        /// <remarks>
        ///      Sample request:
        ///
        ///     GET api/v1/holidays/filter?fromDate=25/07/2021&amp;toDate=31/12/2021
        ///
        /// </remarks>
        /// <param name="fromDate">Data e fillimit te filtrimit ne formatin dd/MM/yyyy</param>
        /// <param name="toDate">Data e mbarimit te filtrimit ne formatin dd/MM/yyyy</param>
        /// <returns>Listen me te dhenat e diteve te pushimit</returns>
        /// <response code="200"> Kthen listen me te dhenat </response>
        /// <response code="400"> Kthen nje mesazh me gabimin e te dhenave te derguara ( formati i dates ose viti i gabuar ) </response>
        /// <response code="500"> Kthen nje mesazh me gabimin qe ka ndodhur gjate marrjes se te dhenave  </response>
        [HttpGet(ApiRoutes.Holidays.GetHolidaysFromDateToDate)]
        [ProducesResponseType(typeof(List<Holiday>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessageResponse),StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorMessageResponse),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHolidaysFromDateToDate([FromQuery ] string fromDate,
            [FromQuery] string toDate)
        {
            if (string.IsNullOrEmpty(fromDate))
                return BadRequest( new ErrorMessageResponse
                {
                    Message = "Provide 'fromDate' value "
                });
            
            if (string.IsNullOrEmpty(toDate))
                return BadRequest( new ErrorMessageResponse
                {
                    Message = "Provide 'toDate' value "
                });

            var validatorResult = "";
            
            try
            {
                validatorResult = await  CheckIfInputDateIsValid(fromDate);

                if (validatorResult != "OK")
                    return BadRequest( new ErrorMessageResponse
                    {
                        Message = validatorResult
                    });
                
                validatorResult = await CheckIfInputDateIsValid(toDate);

                if (validatorResult != "OK")
                    return BadRequest(new ErrorMessageResponse
                    {
                        Message = validatorResult
                    });

            
                // Ketu behet filtrimi ...
                // Logjika ka vend per permiresim...
                var listOfAllHolidays = await _service.GetListOfHolidaysAsync();

                var fromDateAsDateTime = ParseStringToAlbanianDate(fromDate);
                var toDateAsDateTime = ParseStringToAlbanianDate(toDate);
            
                var filteredList = (from holiday in listOfAllHolidays 
                    let holidayDateTime = ParseStringToAlbanianDate(holiday.Day) 
                    where holidayDateTime.Date >= fromDateAsDateTime.Date && holidayDateTime.Date <= toDateAsDateTime.Date 
                    select holiday).ToList();

                return Ok(filteredList);
            
            }
            catch (AggregateException aggregateException)
            {
                var builder = new StringBuilder();

                // catch whatever was thrown
                foreach (var ex in aggregateException.InnerExceptions)
                {
                    builder.AppendLine($"Type: { aggregateException.GetType() },  Message: { aggregateException.Message } {Environment.NewLine}");
                }
                
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMessageResponse
                {
                    Message = builder.ToString()
                }); 
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,  new ErrorMessageResponse
                {
                    Message = exception.Message
                });
            }
        }


        /// <summary>
        ///     Kontrollon nese data e kaluar si queryParam eshte e vlefshme
        ///     Nje date eshte e vlefshme nese ka formatin mm/dd/yyyy dhe viti yyyy eshte sa viti aktual / viti qe shfaq kalendari
        /// </summary>
        /// <param name="inputDate"></param>
        /// <returns>Mesazhin e gabimit</returns>
        private async Task<string> CheckIfInputDateIsValid(string inputDate)
        {
            var calendarYear = await _service.GetYearOfHolidaysAsync();

            var parsed = ParseStringToAlbanianDate(inputDate);
            if (parsed == DateTime.MinValue)
                return "Provided date is not valid format. Allowed format is 'dd/MM/yyyy'";

            if (parsed.Year != calendarYear)
                return "Provided date has not same year as calendar date";

            return "OK";
        }

        private DateTime ParseStringToAlbanianDate(string inputDate)
        {
            var parsed =  DateTime.TryParseExact(inputDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsedDate );
            
            return parsedDate;
        }
    }
}