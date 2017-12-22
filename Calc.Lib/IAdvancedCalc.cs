using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Calc
{
	[ServiceContract]
	public interface IAdvancedCalc
	{
		[OperationContract]
		[WebGet(UriTemplate = "/log/{value}",
					 BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		string Log10(string value);
	}
}
