using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class Base : BaseEntity
    {
        
        public Guid OwnerId { get; set; }
        public bool Archived { get; set; }

        //public DateTime CreatedOn { get; set; }
        //public DateTime UpdatedOn { get; set; }
        public override List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            base.RemapAllGuids(map);
            OwnerId = map(OwnerId, false);
            return null;
        }
    }



    public abstract class BaseEntity
    {
        private bool isDirty;

        public BaseEntity()
        {
            var date = DateTime.Now;
            Id = Guid.NewGuid();
            Created = date;
            Updated = date;
        }
//#if MONGO_DB
//        [BsonId(IdGenerator = typeof(GuidGenerator))]
//#endif
        [Key]
        public Guid Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [NotMapped]
        [XmlIgnore]
        //[BsonIgnore]
        public virtual bool IsDirty { get { return isDirty; } }


        [XmlIgnore]
        [NotMapped]
        //[BsonIgnore]
        public bool IsNew { get { return Created == Updated; } }

        public void MakeDirty(bool dirty = false)
        {
            Updated = DateTime.Now;
            isDirty = dirty;
        }

        public virtual List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            Id = map(Id, false);
            return null;
        }
    }


}
