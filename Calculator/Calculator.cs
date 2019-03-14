using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Calculator
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Calculator : StatelessService
    {
        public Calculator(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[] { new ServiceInstanceListener(context => GetInstance(context)) };
        }

        private WcfCommunicationListener<IService> GetInstance(StatelessServiceContext context)
        {
            var host = context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("wcfEndpoint");
            int port = endpointConfig.Port;
            string schema = endpointConfig.Protocol.ToString();
            string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}", schema, host, port);

            var listener = new WcfCommunicationListener<IService>(
            wcfServiceObject: new Service(),
            serviceContext: context,
            listenerBinding: CreateDefaultHttpBinding(),
            address: new EndpointAddress(uri)
            );

            ServiceMetadataBehavior smb = listener.ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            // If not, add one
            if (smb == null)
            {
                smb = new ServiceMetadataBehavior();
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri(uri);

                listener.ServiceHost.Description.Behaviors.Add(smb);
            }

            return listener;
        }

        private Binding CreateDefaultHttpBinding()
        {
            return new BasicHttpBinding();
        }
    }
}
