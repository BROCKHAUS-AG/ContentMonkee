using System.Collections.Generic;
using BAG.Common.Data;

namespace Default.WebUI.Models
{
    public class GenericSelectorWrapperModel
    {
        public GenericSelectorWrapperModel(string id, List<ISelectable> items, bool isMultiselect = false, string title = "Produkt auswählen")
        {
            ModalId = id;
            Items = items;
            IsMultiselect = isMultiselect;
            Title = title;
        }

        public string ModalId { get; set; }

        public List<ISelectable> Items { get; set; }

        public bool IsMultiselect { get; set; }

        public string Title { get; set; }
    }
}