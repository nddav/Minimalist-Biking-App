using System;
using System.Collections.Generic;
using System.Device.Location;
using System.ServiceModel;
using System.ServiceModel.Web;
using static Lets_go_biking.Service1;

namespace Lets_go_biking
{
    [ServiceContract]
    public interface Interface1
    {
        
        [OperationContract]
        [WebInvoke(UriTemplate = "Itinerary?ORIGIN={origin}&DEST={destination}", BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json)]
        List<Step> getItinerary(string origin,string destination);
   
    }
}
