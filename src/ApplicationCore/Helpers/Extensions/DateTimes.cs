using System;

namespace ApplicationCore.Helpers;
public static class DateTimeHelpers
{
	public static DateTime? ToStartDate(this string input)
	{
		var startDate = input.ToDatetimeOrNull();
		if (startDate.HasValue)
		{
			var dateStart = Convert.ToDateTime(startDate);
			return new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
		}
		else return null;
	}

	public static DateTime? ToEndDate(this string input)
	{
		var endDate = input.ToDatetimeOrNull();
		if (endDate.HasValue)
		{
			var dateEnd = Convert.ToDateTime(endDate);
			return dateEnd.ToEndDate();
		}
		else return null;
	}

	public static DateTime ToEndDate(this DateTime date)
	{
		return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
	}
	
	public static DateTime? ToDatetimeOrNull(this string str)
	{
		if(String.IsNullOrEmpty(str)) return null;

		DateTime dateValue;
		if (DateTime.TryParse(str, out dateValue)) return dateValue;
		return null;
	}
	public static DateTime ToDatetimeOrDefault(this string str, DateTime defaultValue)
	{
		DateTime dateValue;
		if (DateTime.TryParse(str, out dateValue)) return dateValue;

		return defaultValue;

	}
	public static DateTime ToDatetime(this int val)
	{
		var strVal = val.ToString();

		int year = strVal.Substring(0, 4).ToInt();
		int month = strVal.Substring(4, 2).ToInt();
		int day = strVal.Substring(6, 2).ToInt();

		return new DateTime(year, month, day);

	}
	public static string GetDateString(DateTime dateTime)
	{
		string year = dateTime.Year.ToString();
		string month = dateTime.Month.ToString();
		string day = dateTime.Day.ToString();

		if (dateTime.Month < 10) month = "0" + month;
		if (dateTime.Day < 10) day = day = "0" + day;


		return year + month + day;
	}
	static string GetTimeString(DateTime dateTime, bool toMileSecond = false)
	{
		string hour = dateTime.Hour.ToString();
		string minute = dateTime.Minute.ToString();
		string second = dateTime.Second.ToString();
		string mileSecond = dateTime.Millisecond.ToString();

		if (dateTime.Hour < 10) hour = "0" + hour;
		if (dateTime.Minute < 10) minute = "0" + minute;
		if (dateTime.Second < 10) second = "0" + second;

		if (!toMileSecond) return hour + minute + second;


		if (dateTime.Millisecond < 10)
		{
			mileSecond = "00" + mileSecond;
		}
		else if (dateTime.Millisecond < 100)
		{
			mileSecond = "0" + mileSecond;
		}

		return hour + minute + second + mileSecond;

	}
	public static int ToDateNumber(this DateTime input) => Convert.ToInt32(GetDateString(input.Date));
	public static int ToTimeNumber(this DateTime input) => Convert.ToInt32(GetTimeString(input));
	public static string ToDateString(this DateTime input) => input.ToString("yyyy-MM-dd");
	public static string ToDateString(this DateTime? input) => input.HasValue ? input.Value.ToDateString() : string.Empty;
	public static string ToDateTimeString(this DateTime input) => input.ToString("yyyy-MM-dd H:mm:ss");
	public static string ToDateTimeString(this DateTime? input) => input.HasValue ? input.Value.ToDateTimeString() : string.Empty;

}
