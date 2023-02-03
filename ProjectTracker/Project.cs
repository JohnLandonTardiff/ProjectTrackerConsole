using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectTracker
{
    public class Project
    {
        public FileInfo projectFile;

        public List<Goal> goals = new List<Goal>();

        private string name;

        public Project(string name, FileInfo projectFile)
        {
            this.name = name;
            this.projectFile = projectFile;
        }

        public Project(string name, Goal[] goals, FileInfo projectFile)
        {
            this.name = name;
            foreach (Goal goal in goals)
            {
                this.goals.Add(goal);
            }
            this.projectFile = projectFile;
        }

        public void SetName(string newName)
        {
            this.name = newName;
        }

        public string GetName()
        {
            return this.name;
        }

        public void AddGoal(string newGoal)
        {
            goals.Add(new Goal("0", newGoal));
            SaveProject();
        }

        public void DeleteGoal(Goal goal)
        {
            goals.Remove(goal);
            SaveProject();
        }

        public void DeleteProject()
        {
            File.Delete(projectFile.FullName);
        }

        public void SaveProject()
        {
            string fileText = "";
            foreach( Goal goal in goals)
            {
                fileText += goal.SaveGoal() + "\n";
            }
            File.WriteAllText(projectFile.FullName, fileText);
        }

        public override string ToString()
        {
            string output = name + ":\n";
            foreach (Goal goal in goals)
            {
                output += goal.ToString() + "\n";
            }
            return output;
        }
    }
}
