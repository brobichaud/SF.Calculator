using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Calc
{
	[ServiceContract]
	public interface ICalcLib
	{
		[OperationContract]
		[WebGet(UriTemplate = "/round/{value}",
					 BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		string Round(string value);

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "/add/{value1}/{value2}",
					 BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		string Add(string value1, string value2);
	}
}
