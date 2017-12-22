using System;

namespace Calc
{
	public class CalcLib : ICalcLib
	{
		public string Round(string value)
		{
			decimal toRound;
			decimal.TryParse(value, out toRound);
			var rounded = Math.Round(toRound);

			return rounded.ToString();
		}

		public string Add(string value1, string value2)
		{
			int v1, v2;
			int.TryParse(value1, out v1);
			int.TryParse(value2, out v2);
			int result = v1 + v2;

			return result.ToString();
		}
	}
}
