using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class WidgetCarusel : Widget
    {
        public WidgetCarusel()
        {
            Name = "Carousel";
            Items = new List<CaruselItem>();
            WidgetDescription = "An Carousel with Image";
        }

        public List<CaruselItem> Items { get; set; }

        [XmlIgnore]
        public string ItemsString
        {
            get
            {
                var itemList = Items.Select(item => string.Format("{0};{1};{2}", item.Id, item.Image, item.Url)).ToList();
                return string.Join(",", itemList);
            }

            set
            {
                Items.Clear();
                if (value.Contains(','))
                {
                    var itemList = value.Split(',').ToList();
                    foreach (string item in itemList)
                    {
                        var tiles = item.Split(';');
                        var caruselItem = new CaruselItem
                        {
                            Id = Guid.NewGuid(),
                            Image = tiles[1],
                            Url = tiles[2]
                        };

                        Items.Add(caruselItem);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        var tiles = value.Split(';');
                        var caruselItem = new CaruselItem
                        {
                            Id = Guid.NewGuid(),
                            Image = tiles[1],
                            Url = tiles[2]
                        };

                        Items.Add(caruselItem);
                    }
                }
            }
        }

        public override string GetContent()
        {
            return " ";
        }
    }

    public class CaruselItem
    {
        public CaruselItem()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
    }
}
