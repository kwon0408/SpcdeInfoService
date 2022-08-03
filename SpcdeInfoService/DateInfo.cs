using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpcdeInfoService
{
    public class DateInfo
    {
        public DateKind DateKind { get; set; }
        public string DateName { get; set; } = "";

        [Newtonsoft.Json.JsonProperty("isHoliday")]
        public string IsHolidayOriginal { get; set; } = "";
        public bool IsHoliday => IsHolidayOriginal == "Y";

        [Newtonsoft.Json.JsonProperty("kst")]
        private string KstOriginal { get; set; } = "";
        public TimeOnly Kst
        {
            get
            {
                try
                {
                    return TimeOnly.ParseExact(KstOriginal, "hhmm"); 
                }
                catch
                {
                    return TimeOnly.FromTimeSpan(TimeSpan.Zero); 
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("locDate")]
        public int LocDateOriginal { get; set; }
        public DateOnly LocDate => DateOnly.ParseExact(LocDateOriginal.ToString(), "yyyyMMdd");

        public int Seq { get; set; }
        public int SunLongitude { get; set; }

        public static implicit operator DateOnly(DateInfo value)
            => value.LocDate;

        public static implicit operator TimeOnly(DateInfo value)
            => value.Kst;

        public static implicit operator DateTime(DateInfo value)
            => value.LocDate.ToDateTime(value.Kst);

        public override string ToString()
            => $"DateKind = {DateKind}, DateName = {DateName}, IsHoliday = {IsHolidayOriginal}, DateTime = {(DateTime)this:f}, Seq = {Seq}, SunLongitude = {SunLongitude}";
    }
}
