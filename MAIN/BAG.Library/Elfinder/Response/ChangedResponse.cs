using System.Collections.Generic;
using System.Runtime.Serialization;
using BAG.Framework.ElFinder.DTO;

namespace BAG.Framework.ElFinder.Response
{
    [DataContract]
    internal class ChangedResponse
    {
        [DataMember(Name = "changed")]
        public List<FileDTO> Changed { get; private set; }

        public ChangedResponse()
        {
            Changed = new List<FileDTO>();
        }
    }
}