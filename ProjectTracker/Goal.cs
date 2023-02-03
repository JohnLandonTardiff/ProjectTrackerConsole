using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTracker
{
    public class Goal
    {
        public enum goalStatuses
        {
            Not_Yet_Started,
            In_Progress,
            Completed,
            Error
        }

        private goalStatuses status;
        private string name;

        public Goal(string status, string name)
        {
            switch (status)
            {
                case "0":
                    this.status = goalStatuses.Not_Yet_Started;
                    break;
                case "1":
                    this.status = goalStatuses.In_Progress;
                    break;
                case "2":
                    this.status = goalStatuses.Completed;
                    break;
                default:
                    this.status = goalStatuses.Error;
                    break;
            }
            this.name = name;
        }

        public void SetName(string newName)
        {
            this.name = newName;
        }

        public void SetStatus(goalStatuses newStatus)
        {
            this.status = newStatus;
        }

        public string GetName()
        {
            return this.name;
        }

        public goalStatuses GetStatus()
        {
            return this.status;
        }

        public override string ToString()
        {
            return this.name + " - " + this.status.ToString();
        }

        public string SaveGoal()
        {
            string output = "";
            switch (status)
            {
                case goalStatuses.Not_Yet_Started:
                    output = "0";
                    break;
                case goalStatuses.In_Progress:
                    output = "1";
                    break;
                case goalStatuses.Completed:
                    output = "2";
                    break;
            }
            output += name;
            return output;
        }
    }
}
