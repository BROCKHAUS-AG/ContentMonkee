using System.Runtime.Serialization;

namespace BAG.Framework.ElFinder.Response
{
    [DataContract]
    internal class GetResponse
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}