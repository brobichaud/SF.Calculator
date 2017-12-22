using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Calc;

namespace Calculator.Stateless
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class StatelessCalc : StatelessService
	{
		public StatelessCalc(StatelessServiceContext context)
			 : base(context)
		{ }

		/// <summary>
		/// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
		/// </summary>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			return new[] {
						new ServiceInstanceListener((context) => CreateListenerICalc(context), "Calc"),
						new ServiceInstanceListener((context) => CreateListenerIAdvancedCalc(context), "AdvancedCalc")
			};
		}

		private static ICommunicationListener CreateListenerICalc(StatelessServiceContext context)
		{
			var listener = new WcfCommunicationListener<ICalcLib>(
									context,
									new CalcLib(),
									GetWebBinding(),
									GetEndpointAddressICalc(context)
								);

			AddWebBehavior(listener.ServiceHost);

			return listener;
		}

		private static ICommunicationListener CreateListenerIAdvancedCalc(StatelessServiceContext context)
		{
			var listener = new WcfCommunicationListener<IAdvancedCalc>(
									context,
									new AdvancedCalc(),
									GetWebBinding(),
									GetEndpointAddressIAdvancedCalc(context)
								);

			AddWebBehavior(listener.ServiceHost);

			return listener;
		}

		private static EndpointAddress GetEndpointAddressICalc(ServiceContext context)
		{
			var endpoint = context.CodePackageActivationContext.GetEndpoint("CalculatorEndpoint");
			var protocol = endpoint.Protocol;
			int port = endpoint.Port;

			var address = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/calc",
									protocol,
									context.NodeContext.IPAddressOrFQDN,
									port)
							);

			return new EndpointAddress(address, new AddressHeader[0]);
		}

		private static EndpointAddress GetEndpointAddressIAdvancedCalc(ServiceContext context)
		{
			var endpoint = context.CodePackageActivationContext.GetEndpoint("CalculatorEndpoint");
			var protocol = endpoint.Protocol;
			int port = endpoint.Port;

			var address = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/advanced",
									protocol,
									context.NodeContext.IPAddressOrFQDN,
									port)
							);

			return new EndpointAddress(address, new AddressHeader[0]);
		}

		private static WebHttpBinding GetWebBinding()
		{
			var wb = new WebHttpBinding(WebHttpSecurityMode.None);
			//var wb = new WebHttpBinding("clearBinding");
			return wb;
		}

		private static void AddWebBehavior(ServiceHost host)
		{
			var endpoint = host.Description.Endpoints[0];
			endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
		}

		///// <summary>
		///// This is the main entry point for your service instance.
		///// </summary>
		///// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
		//protected override async Task RunAsync(CancellationToken cancellationToken)
		//{
		//	// TODO: Replace the following sample code with your own logic 
		//	//       or remove this RunAsync override if it's not needed in your service.

		//	long iterations = 0;

		//	while (true)
		//	{
		//		cancellationToken.ThrowIfCancellationRequested();

		//		ServiceEventSource.Current.ServiceMessage(this, "Working-{0}", ++iterations);

		//		await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
		//	}
		//}
	}
}
