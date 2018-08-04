using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoOnDemand.Admin.Models
{
    // contains the number of records, the background color of the card, the name of the Glyphicon to display on the card, a description, and the URL to navigate to.
    public class CardViewModel
    {
        public int Count { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string BackgroundColor { get; set; }
    }
}
