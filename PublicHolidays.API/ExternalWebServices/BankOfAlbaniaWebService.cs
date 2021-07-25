using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using PublicHolidays.API.Domain;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace PublicHolidays.API.ExternalWebServices
{
    public class BankOfAlbaniaWebService : IBankOfAlbaniaWebService
    {
        private readonly IConfiguration _configuration;
        
        public BankOfAlbaniaWebService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///     Kthen vitin per te cilin jane publikuar pushimet tek faqjae BSH
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetYearOfHolidaysAsync()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = _configuration.GetValue<string>("BankOfAlbaniaUrl");
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            
            // Hapat per te marre te dhenat nga website i BSH
            
            // 1: Shko tek linku https://www.bankofalbania.org/Shtypi/ dhe ne te majte kerko per linkun qe permban tekstin "Kalendari i festave zyrtare per vitin ..."
            //    Ketu merr linkun qe te con tek kalendari dhe vitin per te cilin jane publikuar keto pushimet ( vlere kjo qe perdoret me vone per validim )

            var documentAnchors = document.QuerySelectorAll( "a" );

            var calendarLink = documentAnchors.FirstOrDefault(x =>
                x.InnerHtml.Contains("Kalendari i festave zyrtare për vitin", StringComparison.OrdinalIgnoreCase)) as AngleSharp.Html.Dom.IHtmlAnchorElement;

            return int.Parse( Regex.Match(calendarLink.Text(), @"\d+").Value );
        }
        
        
        public async Task<IEnumerable<Holiday>> GetListOfHolidaysAsync()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = _configuration.GetValue<string>("BankOfAlbaniaUrl");
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            
            // Hapat per te marre te dhenat nga website i BSH
            
            // 1: Shko tek linku https://www.bankofalbania.org/Shtypi/ dhe ne te majte kerko per linkun qe permban tekstin "Kalendari i festave zyrtare per vitin ..."
            //    Ketu merr linkun qe te con tek kalendari dhe vitin per te cilin jane publikuar keto pushimet ( vlere kjo qe perdoret me vone per validim )

            var documentAnchors = document.QuerySelectorAll( "a" );

            var calendarLink = documentAnchors.FirstOrDefault(x =>
                x.InnerHtml.Contains("Kalendari i festave zyrtare për vitin", StringComparison.OrdinalIgnoreCase)) as AngleSharp.Html.Dom.IHtmlAnchorElement;
            
            // 2: Krijon linkun dhe ngarkon faqen qe permban kalendarin
            var calendarUrl = $"{calendarLink?.BaseUrl.Origin}{calendarLink?.Attributes["href"].Value}";

            var calendarDocument = await context.OpenAsync(calendarUrl);

            if (calendarDocument == null)
                return new List<Holiday>();
            
            // Merr paragrafet qe permbajne shenimet
            var notesParagraph = calendarDocument.QuerySelectorAll( "p" ).OfType<IHtmlParagraphElement>();

            var htmlParagraphElements = notesParagraph as IHtmlParagraphElement[] ?? notesParagraph.ToArray();
            
            // Merr tekstin e paragrafeve
            var firstNoteText = CleanStringValues( htmlParagraphElements.FirstOrDefault( x => FirstNoteParagraphText( x.Text() ) )?.Text() );
            var secondNoteText = CleanStringValues( htmlParagraphElements.FirstOrDefault( x => SecondNoteParagraphText( x.Text() ) )?.Text() );	
            var thirdNoteText = CleanStringValues( htmlParagraphElements.FirstOrDefault( x => ThirdNoteParagraphText( x.Text() ) )?.Text() );	
            
            
            // Merr tabelen me te dhenat e publikuara
            var holidaysDataRows =  calendarDocument.QuerySelectorAll( "table.datatable > tbody > tr" ).OfType<IHtmlTableRowElement>();
            
            // Nderton objektin Holiday duke marre vlerat e qelizave te rreshtit
            var listOfHolidays = holidaysDataRows.Select(x => new
            Holiday{
                Day = FormatDateToAlbanianFormat( CleanStringValues(x.Cells?.FirstOrDefault()?.Text()) ),
                Name = CleanStringValues(x.Cells?.LastOrDefault()?.Text())
            }).ToList();
            
            // Shton komentet perkatese per cdo dite pushimi
            foreach (var holiday in listOfHolidays)
            {   
                // Komenti qe dita e pushimit bie ne fundjave
                if (holiday.Name.Count(x => x == '*') == 1)
                    holiday.Note = firstNoteText;
                
                // Komenti per festen e Bajramit
                if (holiday.Name.Count(x => x == '*') == 2)
                    holiday.Note = secondNoteText;

                holiday.Name = holiday.Name.Replace( "*", String.Empty);
                
                // Cdo feste le te permbaje komentin e heres se fundit qe eshte bere update!
                holiday.LastUpdateDate += $"{thirdNoteText}";
            }

            return listOfHolidays;
        }

        private string FormatDateToAlbanianFormat(string inputDate)
        {
            if (string.IsNullOrEmpty(inputDate)) return string.Empty;

            var cultureInfo = new CultureInfo("sql-AL");

            DateTime.TryParseExact(inputDate, "dd.mm.yyyy", cultureInfo, DateTimeStyles.None, out var parsedDatetime);

            return parsedDatetime.ToString("dd/mm/yyyy", cultureInfo);
        }
        private string CleanStringValues(string valueToClean)
        {
            if (string.IsNullOrEmpty(valueToClean)) return string.Empty;

            return  Regex.Replace(valueToClean, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline).Trim();
        }
        
        private bool FirstNoteParagraphText( [NotNull] string inputValue ) => inputValue.Contains( "*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.", StringComparison.OrdinalIgnoreCase );
		
        private bool SecondNoteParagraphText( [NotNull] string inputValue ) => inputValue.Contains( "**Shënim: Data e festave të Kurban Bajramit", StringComparison.OrdinalIgnoreCase );
		
        private bool ThirdNoteParagraphText( [NotNull] string inputValue ) => inputValue.Contains( "Përditësuar", StringComparison.OrdinalIgnoreCase );
    }
}