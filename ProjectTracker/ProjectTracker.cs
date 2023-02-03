using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProjectTracker
{
    class ProjectTracker
    {
        //Path for the directory the projects are stored in
        static string projectDirectoryPath;

        //List of Project Objects created from Project text files
        static List<Project> projects = new List<Project>();

        //String constants for Different Menu Options
        const string EXIT_STRING = "EXIT";
        const string BACK_STRING = "BACK";
        const string ADD_STRING = "ADD";
        const string EDIT_STRING = "EDIT";
        const string UPDATE_STRING = "UPDATE";
        const string DELETE_STRING = "DELETE";

        static void Main(string[] args)
        {
            //Read already existing project files
            Startup();

            //Start The process of managing projects
            string input = "";
            while(input != EXIT_STRING)
            {
                input = ShowProjectsMenu();
            }
        }

        //Method for reading already existing project files
        private static void Startup()
        {
            //Get the project directory if it exists and create it if it doesn't
            projectDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Projects");
            DirectoryInfo projectDirectory = Directory.CreateDirectory(projectDirectoryPath);

            //Read project files and store their paths in an array
            string[] projectFilePaths = Directory.GetFiles(projectDirectoryPath, "*.txt").Where(item => item.EndsWith(".txt")).ToArray();

            //Loop through the file paths array if there are any files and map them to project objects
            if (projectFilePaths.Length > 0)
            {
                foreach (string file in projectFilePaths)
                {
                    FileInfo newFile = new FileInfo(file);
                    AddProject(newFile.Name.Substring(0, newFile.Name.Length - 4), ReadGoals(newFile), newFile);
                }
            }
        }

        //Method for reading and returning a list of goals stored inside project files
        private static Goal[] ReadGoals(FileInfo projectFile)
        {
            //loop through the files text lines and map them into goal objects
            List<Goal> goals = new List<Goal>();
            foreach (string goal in File.ReadLines(projectFile.FullName))
            {
                goals.Add(new Goal(goal.Substring(0, 1), goal.Substring(1)));
            }
            return goals.ToArray();
        }

        //Method for showing and navigating through the projects menu
        private static string ShowProjectsMenu()
        {
            //Clear the console and display the projects menu
            Console.Clear();
            string output = "PROJECTS:\n";
            int counter = 0;
            foreach (Project project in projects)
            {
                output += counter + " - " + project.GetName() + "\n";
                counter++;
            }
            output += "Enter the project number, ADD to add a new project, or EXIT to end the program";
            Console.WriteLine(output);

            //Navigate through the projects menu using string input
            int projectIndex;
            string? input = Console.ReadLine();

            //Add a new project if the user enters ADD
            if (input == ADD_STRING)
            {
                AddProject();
            }
            //Enter the current projects goals menu when the user enters a number input
            else if (int.TryParse(input, out projectIndex))
            {
                if(projectIndex < projects.Count())
                {
                    Project project = projects[projectIndex];
                    string goalInput = "";
                    while (goalInput != BACK_STRING)
                    {
                        goalInput = ShowGoalsMenu(project);
                    }
                }
            }
            //return input to check if the user entered EXIT or not
            return input;
        }

        //Method for showing and navigating through a project's goal menu
        private static string ShowGoalsMenu(Project project)
        {
            //Clear the console and display the project's goal menu
            Console.Clear();
            string output = project.GetName().ToUpper() + ":\n";
            int counter = 0;
            foreach (Goal goal in project.goals)
            {
                output += counter + " - " + goal.ToString() + "\n";
                counter++;
            }
            output += "Enter the goal number, ADD to add a new goal, DELETE to delete the project, EDIT to change the project name, or BACK to go back to projects";
            Console.WriteLine(output);

            //Navigate through the project's goal menu
            int goalIndex;
            string? input = Console.ReadLine();

            //Add a new goal if the user enters ADD
            if (input == ADD_STRING)
            {
                AddGoal(project);
            }
            //Show the goal update menu if the user enters an index of the project's goals list
            else if (int.TryParse(input, out goalIndex))
            {
                if (goalIndex < project.goals.Count())
                {
                    Goal goal = project.goals[goalIndex];
                    string goalInput = "";
                    while (goalInput != BACK_STRING)
                    {
                        goalInput = ShowGoalUpdateMenu(project, goal);
                    }
                }
            }
            //Change the project's name if the user enters EDIT
            else if(input == EDIT_STRING)
            {
                EditProject(project);
            }
            //Delete the project if the user enters DELETE
            else if (input == DELETE_STRING)
            {
                input = DeleteProject(project);
            }

            //Return input to check if the user entered BACK
            return input;
        }

        //Method for showing and navigating through a goal's update menu
        private static string ShowGoalUpdateMenu(Project project, Goal goal)
        {
            //Clear the console and display the goal's update menu
            Console.Clear();
            string output = project.GetName().ToUpper() + " - " + goal.GetName().ToUpper() + " - " + goal.GetStatus().ToString().ToUpper() + ":\n";
            output += "Enter UPDATE to update the goal status, \nEDIT to edit the goal name, \nDELETE to delete the goal, \nor BACK to go back to the goals menu";
            Console.WriteLine(output);

            //Navigate through the goal's update menu
            string input = Console.ReadLine();
            switch (input)
            {
                //Update the goal's status if the user enters UPDATE
                case UPDATE_STRING:
                    UpdateGoal(project, goal);
                    break;
                //Edit the goal's name if the user enters EDIT
                case EDIT_STRING:
                    EditGoal(project, goal);
                    break;
                //Delete the goal if the user enters DELETE
                case DELETE_STRING:
                    input = DeleteGoal(project, goal);
                    break;
            }

            //return input to check if the player entered BACK
            return input;
        }

        //Method to prompt the user to enter a name for the new project and creates a new project
        private static void AddProject()
        {
            Console.Clear();
            string? input = null;
            while (string.IsNullOrEmpty(input))
            {
                //Clear the console and prompt the user for the new project's name
                Console.WriteLine("Enter the new project name, or BACK to cancel");

                //Read the user's input and create a new project file based on the name entered
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    if(projects.Find(x => x.GetName() == input) != null)
                    {
                        Console.Clear();
                        Console.WriteLine("Name already exists");
                        input = null;
                        continue;
                    }
                    if(input == BACK_STRING)
                    {
                        break;
                    }
                    string projectFile = Path.Combine(projectDirectoryPath, input + ".txt");
                    FileStream create = File.Create(projectFile);
                    projects.Add(new Project(input, new FileInfo(projectFile)));
                    create.Close();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Name cannot be empty");
                    continue;
                }
            }
        }

        //Method for creating projects based on the files read at startup
        private static void AddProject(string name, Goal[] goals, FileInfo projectFile)
        {
            projects.Add(new Project(name, goals, projectFile));
        }

        //Method for prompting the user to enter a new name for the project
        private static void EditProject(Project project)
        {
            Console.Clear();
            string? input = null;

            while (string.IsNullOrEmpty(input))
            {
                //Clear the console and prompt the user for the goal's new name
                Console.WriteLine("Enter the project's new name, or BACK to cancel");
                input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    Project find = projects.Find(x => x.GetName() == input);
                    if (find != null && find != project)
                    {
                        Console.Clear();
                        Console.WriteLine("Name already exists");
                        input = null;
                        continue;
                    }
                    if(input == BACK_STRING)
                    {
                        input = project.GetName();
                    }

                    project.SetName(input);
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Name cannot be empty");
                    continue;
                }
            }
        }

        //Method for prompting the user if they want to delete the project and deletes it if yes and doesn't if no
        private static string DeleteProject(Project project)
        {
            //Clear the console and prompt the user
            Console.Clear();
            Console.WriteLine("Are you sure you want to delete project:" + project.GetName() + " Y/N?");

            //Read the user's input to confirm the player wants to delete the project
            string? input = Console.ReadLine();
            switch (input)
            {
                case "Y":
                    project.DeleteProject();
                    projects.Remove(project);
                    input = BACK_STRING;
                    break;
                case "N":
                    input = "";
                    break;
            }
            return input;
        }

        //Method for prompting the user to input the name of a new goal and creating it
        private static void AddGoal(Project project)
        {
            Console.Clear();
            string? input = null;

            while (string.IsNullOrEmpty(input))
            {
                //clear the console and prompt the user to enter the new goal's name
                Console.WriteLine("Enter the new goal name, or BACK to cancel");
                input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    if (project.goals.Find(x => x.GetName() == input) != null)
                    {
                        Console.Clear();
                        Console.WriteLine("Name already exists");
                        input = null;
                        continue;
                    }
                    if(input == BACK_STRING)
                    {
                        break;
                    }
                    project.AddGoal(input);
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Name cannot be empty");
                    continue;
                }
            }
        }

        //Method for prompting the user for what the goal's new status should be
        private static void UpdateGoal(Project project, Goal goal)
        {
            //Clear the console and prompt the user to enter the goal's new status
            Console.Clear();
            string? input = null;
            Goal.goalStatuses status = Goal.goalStatuses.Error;

            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Enter the new goal status: \n'0' for 'not yet started'\n'1' for 'in progress'\n'2' for 'completed'\nBACK to cancel");

                //Read the user's input for a new status and set the goals new status
                input = Console.ReadLine();
                switch (input)
                {
                    case "0":
                        status = Goal.goalStatuses.Not_Yet_Started;
                        break;
                    case "1":
                        status = Goal.goalStatuses.In_Progress;
                        break;
                    case "2":
                        status = Goal.goalStatuses.Completed;
                        break;
                    case BACK_STRING:
                        status = goal.GetStatus();
                        break;
                    default:
                        input = null;
                        Console.Clear();
                        Console.WriteLine("Invalid input");
                        continue;
                }
            }

            goal.SetStatus(status);
            project.SaveProject();
        }

        //Method for prompting the user to enter a new name for the goal
        private static void EditGoal(Project project, Goal goal)
        {
            Console.Clear();
            string? input = null;

            while (string.IsNullOrEmpty(input))
            {
                //Clear the console and prompt the user for the goal's new name
                Console.WriteLine("Enter the goal's new name or BACK to cancel");
                input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    Goal find = project.goals.Find(x => x.GetName() == input);
                    if (find != null && find != goal)
                    {
                        Console.Clear();
                        Console.WriteLine("Name already exists");
                        input = null;
                        continue;
                    }
                    if(input == BACK_STRING)
                    {
                        input = goal.GetName();
                    }

                    goal.SetName(input);
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Name cannot be empty");
                    continue;
                }
            }
        }


        //Method for prompting the user to confirm they want to delete the goal
        private static string DeleteGoal(Project project, Goal goal)
        {
            //Clear the console and prompt the user to confirm they want to delete the goal
            Console.Clear();
            Console.WriteLine("Are you sure you want to delete goal:" + goal.GetName() + " Y/N?");

            //Read the user's input for confirming the goal's deletion
            string? input = Console.ReadLine();
            switch (input)
            {
                case "Y":
                    project.DeleteGoal(goal);
                    input = BACK_STRING;
                    break;
                case "N":
                    input = "";
                    break;
            }
            return input;
        }
    }
}
