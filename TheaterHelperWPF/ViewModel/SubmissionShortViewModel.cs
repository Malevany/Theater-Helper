using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelperWPF.ViewModel
{
    public class SubmissionShortViewModel : ViewModelBase
    {
        public ScheduledSubmission scheduledSubmission;

        public SubmissionShortViewModel(ScheduledSubmission scheduledSubmission)
        {
            this.scheduledSubmission = scheduledSubmission;
        }
        public int ID => scheduledSubmission.Id;
        public string Date
        {
            get => $"{scheduledSubmission.Session.SessionDate:M} {scheduledSubmission.Session.SessionDate:t}";
        }
        public string SubmissionName
        {
            get => scheduledSubmission.Submission.Play.Name;
        }
        public string SessionState
        {
            get => scheduledSubmission.Session.GetStatus();
        }
        public string SessionStateColor
        {
            get
            {
                if (SessionState == "Предстоит")
                {
                    return "DeepSkyBlue";
                }
                else if (SessionState == "Отменена")
                {
                    return "Red";
                }
                else
                {
                    return "Green";
                }
            }
        }
    }
}
