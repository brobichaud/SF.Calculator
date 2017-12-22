using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc
{
	public class AdvancedCalc : IAdvancedCalc
	{
		public string Log10(string value)
		{
			double toLog;
			double.TryParse(value, out toLog);
			var logged = Math.Log10(toLog);

			return logged.ToString();
		}
	}
}
