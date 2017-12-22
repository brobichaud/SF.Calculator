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

namespace Calculator.Stateful
{
	/// <summary>
	/// An instance of this class is created for each service replica by the Service Fabric runtime.
	/// </summary>
	internal sealed class StatefulCalc : StatefulService
	{
		public StatefulCalc(StatefulServiceContext context)
			 : base(context)
		{ }

		/// <summary>
		/// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
		/// </summary>
		/// <remarks>
		/// For more information on service communication, see http://aka.ms/servicefabricservicecommunication
		/// </remarks>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			return new[] {
					 new ServiceReplicaListener((context) => CreateWcfWebCommunicationListener(context))
			};
		}

		private static ICommunicationListener CreateWcfWebCommunicationListener(StatefulServiceContext context)
		{
			var listener = new WcfCommunicationListener<ICalcLib>(
								 context,
								 new CalcLib(),
								 GetWebBinding(),
								 GetListenEndpointAddress(context)
			);

			var endpoint = listener.ServiceHost.Description.Endpoints[0];
			endpoint.EndpointBehaviors.Add(new WebHttpBehavior());

			return listener;
		}

		private static EndpointAddress GetListenEndpointAddress(ServiceContext context)
		{
			var endpoint = context.CodePackageActivationContext.GetEndpoint("CalculatorEndpoint");
			var protocol = endpoint.Protocol;
			int port = endpoint.Port;

			return new EndpointAddress(GetListenAddress(context, protocol.ToString(), port), new AddressHeader[0]);
		}

		private static Uri GetListenAddress(ServiceContext context, string scheme, int port)
		{
			return new Uri(
				string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}",
						scheme,
						context.NodeContext.IPAddressOrFQDN,
						port)
			);
		}

		private static WebHttpBinding GetWebBinding()
		{
			var wb = new WebHttpBinding(WebHttpSecurityMode.None);
			//var wb = new WebHttpBinding("clearBinding");
			return wb;
		}

		///// <summary>
		///// This is the main entry point for your service replica.
		///// This method executes when this replica of your service becomes primary and has write status.
		///// </summary>
		///// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
		//protected override async Task RunAsync(CancellationToken cancellationToken)
		//{
		//	// TODO: Replace the following sample code with your own logic 
		//	//       or remove this RunAsync override if it's not needed in your service.

		//	var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

		//	while (true)
		//	{
		//		cancellationToken.ThrowIfCancellationRequested();

		//		using (var tx = this.StateManager.CreateTransaction())
		//		{
		//			var result = await myDictionary.TryGetValueAsync(tx, "Counter");

		//			ServiceEventSource.Current.ServiceMessage(this, "Current Counter Value: {0}",
		//				 result.HasValue ? result.Value.ToString() : "Value does not exist.");

		//			await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

		//			// If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
		//			// discarded, and nothing is saved to the secondary replicas.
		//			await tx.CommitAsync();
		//		}

		//		await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
		//	}
		//}
	}
}
