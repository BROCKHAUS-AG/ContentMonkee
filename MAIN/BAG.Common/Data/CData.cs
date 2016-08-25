using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BAG.Common.Data
{

    public class CData : IXmlSerializable
    {
        public string Value { get; set; }

        public static CData Empty = string.Empty;

        public static implicit operator CData(string value)
        {
            return new CData(value);
        }

        public static implicit operator string(CData cdata)
        {
            return cdata == null ? null : cdata.Value;
        }

        public CData() : this(string.Empty)
        {
        }

        public CData(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            Value = reader.ReadElementString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteCData(Value);
        }
        public string ToUpper()
        {
            return this.Value.ToUpper();
        }
        public string ToLower()
        {
            return this.Value.ToLower();
        }
    }
}
