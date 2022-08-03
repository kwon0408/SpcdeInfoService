using Newtonsoft.Json;

namespace SpcdeInfoService
{
    public enum DateKind
    {
        /// <summary>국경일</summary>
        NationalHoliday = 1,
        /// <summary>기념일</summary>
        Anniversary,
        /// <summary>24절기</summary>
        TwentyFourDivisions,
        /// <summary>잡절</summary>
        SundryDay
    }
}