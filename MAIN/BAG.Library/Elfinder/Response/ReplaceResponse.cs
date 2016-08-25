using System.Collections.Generic;
using System.Runtime.Serialization;
using BAG.Framework.ElFinder.DTO;

namespace BAG.Framework.ElFinder.Response
{
    [DataContract]
    internal class ReplaceResponse
    {
        [DataMember(Name = "added")]
        public List<DTOBase> Added { get; private set; }

        [DataMember(Name = "removed")]
        public List<string> Removed { get; private set; }

        public ReplaceResponse()
        {
            Added = new List<DTOBase>();
            Removed = new List<string>();
        }
    }
}