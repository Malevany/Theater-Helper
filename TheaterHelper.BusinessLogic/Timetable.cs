using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Timetable
    {
        private List<ScheduledSubmission> schedule;

        public List<ScheduledSubmission> Schedule { get { return schedule; } }

        public Timetable(List<ScheduledSubmission> schedule)
        {
            this.schedule = schedule;
        }
    }
}
