using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class WidgetGrid : Widget
    {
        public WidgetGrid()
            : base()
        {
            Name = "Grid";
            WidgetDescription = "Widget with a Grid system";
        }

        public bool ThreeCol { get; set; }
        public bool TwoColLeft { get; set; }
        public bool TwoColRight { get; set; }
        public string ButtonClass { get; set; }

        public string ThreeColFirstTitle { get; set; }
        public string ThreeColFirstSubTitle { get; set; }
        public string ThreeColFirstDesc { get; set; }
        public string ThreeColFirstLink { get; set; }
        public string ThreeColFirstFile { get; set; }
        public bool ThreeColFirstIconUse { get; set; }
        public string ThreeColFirstIcon { get; set; }

        public string ThreeColSecondTitle { get; set; }
        public string ThreeColSecondSubTitle { get; set; }
        public string ThreeColSecondDesc { get; set; }
        public string ThreeColSecondLink { get; set; }
        public string ThreeColSecondFile { get; set; }
        public bool ThreeColSecondIconUse { get; set; }
        public string ThreeColSecondIcon { get; set; }

        public string ThreeColThirdTitle { get; set; }
        public string ThreeColThirdSubTitle { get; set; }
        public string ThreeColThirdDesc { get; set; }
        public string ThreeColThirdLink { get; set; }
        public string ThreeColThirdFile { get; set; }
        public bool ThreeColThirdIconUse { get; set; }
        public string ThreeColThirdIcon { get; set; }

        public string TwoColLeftTitle { get; set; }
        public string TwoColLeftDesc { get; set; }
        public string TwoColLeftLink { get; set; }
        public string TwoColLeftFile { get; set; }
        public bool TwoColLeftIconUse { get; set; }
        public string TwoColLeftIcon { get; set; }

        public string TwoColRightTitle { get; set; }
        public string TwoColRightDesc { get; set; }
        public string TwoColRightLink { get; set; }
        public string TwoColRightFile { get; set; }
        public bool TwoColRightIconUse { get; set; }
        public string TwoColRightIcon { get; set; }


        public override string GetContent()
        {
            return ThreeColFirstTitle + " "
                + ThreeColFirstDesc + " "
                + ThreeColSecondTitle + " "
                + ThreeColSecondDesc + " "
                + ThreeColThirdTitle + " "
                + ThreeColThirdDesc + " ";
        }
    }
}
