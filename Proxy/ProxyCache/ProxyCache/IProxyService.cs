using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using static Proxy.ProxyService;

namespace ProxyCache
{
    [ServiceContract]
    internal interface IProxyService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Stations?CITY={city}", BodyStyle = WebMessageBodyStyle.Wrapped, Method = "REST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Json)]
        List<JCDStation> getStationsFromProxy(string city);
    }

    [DataContract]
    [Serializable]
    public class JCDStation
    {
        [DataMember]
        public int number { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Position position { get; set; }
        [DataMember]
        public int available_bikes { get; set; }
        [DataMember]
        public int available_bike_stands { get; set; }
        [DataMember]
        public string status { get; set; }
    }
}
