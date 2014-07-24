using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Simulation.WorldState;

namespace Veis.Workflow.YAWL
{
    public static class YAWLFormattingExtensions
    {
        public static List<string> ExtractTasks(this string taskString)
        {
            var separated = taskString.Split(' ').ToList();
            return separated.Select(s => s.Replace('_', ' ')).ToList();
        }

        public static string FormatWorkitemName(this WorkItem workitem)
        {
            var lastUnderscore = workitem.taskName.LastIndexOf('_');
            var formattedString = workitem.taskName.Remove(lastUnderscore).Replace('_', ' ');
            return formattedString;
        }

        /// <summary>
        /// Work item goals should be structured as
        // <Asset_Name>:<Predicate>;<Value>+
        // <Asset_Name>:<Predicate>;<Value>+
        // etc...
        /// </summary>
        public static Goal ExtractGoal(this string goalString)
        {
            const string GoalStateSeparator = "+";
            const string PartSeparator = ":;";
            const int RequiredNumberOfParts = 3;
            
            Goal newGoal = new Goal();
            var goalStates = goalString.Split(GoalStateSeparator.ToCharArray());

            foreach (var state in goalStates)
            {
                var parts = state.Split(PartSeparator.ToCharArray(), 3);
                if (parts.Count() < RequiredNumberOfParts) continue;

                State goalState = new State
                {
                    Asset = parts[0].Replace('_', ' '),
                    Predicate = PredicateDeUnderscore(parts[1]),
                    Value = parts[2].Replace('_', ' ')
                };
                newGoal.GoalStates.Add(goalState);
            }

            return newGoal;
        }

        /// <summary>
        /// Special deunderscore function for predicates, because some predicates
        /// have an underscore at the front which is used in planning function.
        /// Underscores other than this are replaced with whitespace
        /// </summary>
        public static string PredicateDeUnderscore(string input)
        {
            if (input.StartsWith("_"))
            {
                var text = input.Substring(1).Replace('_', ' ');
                return String.Format("_{0}", text);
            }
            else
            {
                return input.Replace('_', ' ');
            }
        }
           
    }
}
