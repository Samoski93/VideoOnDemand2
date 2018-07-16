using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoOnDemand.UI.Models.DTOModels;

namespace VideoOnDemand.UI.Models.MembershipViewModels
{
    public class DashboardViewModel
    {
        // a list in a list - display three course panels on each row.
        public List<List<CourseDTO>> Courses { get; set; }
    }
}
