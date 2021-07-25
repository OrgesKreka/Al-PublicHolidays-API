using System;
using System.Security.Permissions;

namespace PublicHolidays.API.Domain
{
    public class Holiday
    {
        /// <summary>
        ///     Data e festes ne formatin dd/MM/yyyy
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        ///     Emri i festes
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Nese festa bie gjate fundjaves ose nese eshte festa e Bajramit ka shenime
        ///     qe tregojne se duhet te merret dhe e hena pushim ose nese ka ndryshime sipas kalendarit henor
        /// </summary>
        public string Note { get; set; }
        
        /// <summary>
        ///     Data e fundit qe eshte bere update i kalendarit tek website i BSH
        /// </summary>
        public string LastUpdateDate { get; set; }
    }
}