using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class Redirection
    {
        public string oldSEOUrl { get; set; }
        public string newSEOUrl { get; set; }
        public Guid Id { get; set; }
    }
}
