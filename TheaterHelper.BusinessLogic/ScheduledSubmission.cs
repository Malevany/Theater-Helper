using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class ScheduledSubmission
    {
        private int id;
        private Session session;
        private Submission submission;

        public int Id { get { return id; } }
        public Session Session { get { return session; } }
        public Submission Submission { get { return submission; } }

        public ScheduledSubmission(int id, Session session, Submission submission)
        {
            this.id = id;
            this.session = session;
            this.submission = submission;
        }
    }
}
