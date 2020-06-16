using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Objects
{
    [Serializable]
    public class UserAction
    {
        public int Day { get; set; }

        public string Action { get; set; }

        public Guid UserID { get; set; }

        public int ProductID { get; set; }

        public UserAction(int day, string action, Guid userid, int productid)
        {
            Day = day;
            Action = action;
            UserID = userid;
            ProductID = productid;
        }

        public override string ToString()
        {
            return Day + "," + Action + "," + UserID + ","  + ProductID;
        }
    }
}
