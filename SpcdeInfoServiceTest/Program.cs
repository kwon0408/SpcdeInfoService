using SpcdeInfoService;

namespace SpcdeInfoServiceTest;
public partial class Program
{
    public static void Main()
    {
        SpcdeInfo info = new(YourApiKey);
        
        // 2018년부터는 토왕용사는 나오지 않음
        int year = 2023;
        int? month = null;

        var holy = info.GetHoliDeInfo(out var total, year, month, 100);
        Console.WriteLine($"------- 국경일: {total}");
        foreach (var day in holy)
            Console.WriteLine(day.ToString());
        Console.WriteLine("");

        var rest = info.GetRestDeInfo(out total, year, month, 100);
        Console.WriteLine($"------- 공휴일: {total}");
        foreach (var day in rest)
            Console.WriteLine(day.ToString());
        Console.WriteLine("");

        var anniv = info.GetAnniversaryInfo(out total, year, month, 100);
        Console.WriteLine($"------- 기념일: {total}");
        foreach (var day in anniv)
            Console.WriteLine(day.ToString());
        Console.WriteLine("");

        var div24 = info.Get24DivisionsInfo(out total, year, month, 100);
        Console.WriteLine($"------- 24절기: {total}");
        foreach (var day in div24)
            Console.WriteLine(day.ToString());
        Console.WriteLine("");

        var sundry = info.GetSundryDayInfo(out total, year, month, 100);
        Console.WriteLine($"------- 잡절: {total}");
        foreach (var day in sundry)
            Console.WriteLine(day.ToString());
        Console.WriteLine("");
    }
}