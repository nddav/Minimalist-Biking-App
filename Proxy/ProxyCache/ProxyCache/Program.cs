using Proxy;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ProxyCache
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //Create a URI to serve as the base address
            Uri httpUrl = new Uri("http://localhost:8090/MyService/ProxyService");

            //Create ServiceHost
            ServiceHost host = new ServiceHost(typeof(ProxyService), httpUrl);

            //Add a service endpoint
            host.AddServiceEndpoint(typeof(IProxyService), new BasicHttpBinding(), "");

            //Enable metadata exchange
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            //Start the Service
            host.Open();

            Console.WriteLine("Service is host at " + DateTime.Now.ToString());

            Console.WriteLine("\nProxy is running... Press <Enter> key to stop");
            Console.ReadLine();
        }
    }
}
