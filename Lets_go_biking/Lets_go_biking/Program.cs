using Lets_go_biking.Proxy;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.ServiceModel;
using System.ServiceModel.Description;
using static Lets_go_biking.Service1;

namespace Lets_go_biking
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //Create a URI to serve as the base address
            Uri httpUrl = new Uri("http://localhost:8090/MyService/Service1");


            //Create ServiceHost
            ServiceHost host = new ServiceHost(typeof(Service1), httpUrl);

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize=1000000;

            //Add a service endpoint
            host.AddServiceEndpoint(typeof(Interface1), new BasicHttpBinding(), "");

            //Enable metadata exchange
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            //Start the Service
            host.Open();

            Console.WriteLine("Service is host at " + DateTime.Now.ToString());    
          

            Console.WriteLine("Rounting server is running... Press <Enter> key to stop");
            Console.ReadLine();
        }
    }
}
