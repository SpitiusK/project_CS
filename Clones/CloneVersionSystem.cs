using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Clones
{
    public class CloneVersionSystem : ICloneVersionSystem
    {
        public Dictionary<string, Clone> AllClones { get; } = new Dictionary<string, Clone>();

        public string Execute(string query)
        {
            var currentQuery = new Query(query);
            if (!AllClones.ContainsKey("1"))
            {
                var firstClone = new Clone(this, "1");
            }
            switch (currentQuery.Action)
            {
                case "learn":
                    return HandleLearn(currentQuery.Name, currentQuery.ProgramStudy);
                case "check":
                    return HandleCheck(currentQuery.Name);
                case "rollback":
                    return HandleRollback(currentQuery.Name);
                case "relearn":
                    return HandleRelearn(currentQuery.Name);
                case "clone":
                    return HandleClone(currentQuery.Name);
                default:
                    return null;
            }
        }

        private string HandleLearn(string cloneName, string programStudy)
        {
            if (AllClones.TryGetValue(cloneName, out var clone))
            {
                clone.RollbackHistory = ImmutableStack<string>.Empty;
            }
            else
            {
                clone = new Clone(this, cloneName);
            }
            clone.StudiedPrograms = clone.StudiedPrograms.Push(programStudy);
            return null;
        }

        private string HandleCheck(string cloneName)
        {
            return AllClones.TryGetValue(cloneName, out var clone) && !clone.StudiedPrograms.IsEmpty
                ? clone.StudiedPrograms.Peek()
                : null;
        }

        private string HandleRollback(string cloneName)
        {
            if (AllClones.TryGetValue(cloneName, out var clone) && !clone.StudiedPrograms.IsEmpty)
            {
                string lastProgram = clone.StudiedPrograms.Peek();
                clone.RollbackHistory = clone.RollbackHistory.Push(lastProgram);
                clone.StudiedPrograms = clone.StudiedPrograms.Pop();
            }
            return null;
        }

        private string HandleRelearn(string cloneName)
        {
            if (AllClones.TryGetValue(cloneName, out var clone) && !clone.RollbackHistory.IsEmpty)
            {
                string lastRollback = clone.RollbackHistory.Peek();
                clone.StudiedPrograms = clone.StudiedPrograms.Push(lastRollback);
                clone.RollbackHistory = clone.RollbackHistory.Pop();
            }
            return null;
        }

        private string HandleClone(string originalCloneName)
        {
            if (AllClones.TryGetValue(originalCloneName, out var originalClone))
            {
                var newClone = new Clone(this, originalClone);
            }
            return null;
        }
    }

    public class Query
    {
        public string Action { get; set; }
        public string Name { get; set; }
        public string ProgramStudy { get; set; }

        public Query(string query)
        {
            var queryArray = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            switch (queryArray[0])
            {
                case "learn":
                    ParseLearn(queryArray);
                    break;
                case "check":
                    ParseCheck(queryArray);
                    break;
                case "rollback":
                    ParseRollback(queryArray);
                    break;
                case "relearn":
                    ParseRelearn(queryArray);
                    break;
                case "clone":
                    ParseClone(queryArray);
                    break;
            }
        }

        private void ParseLearn(string[] queryArray)
        {
            Action = queryArray[0];
            Name = queryArray[1];
            ProgramStudy = queryArray[2];
        }

        private void ParseCheck(string[] queryArray)
        {
            Action = queryArray[0];
            Name = queryArray[1];
        }

        private void ParseRollback(string[] queryArray)
        {
            Action = queryArray[0];
            Name = queryArray[1];
        }

        private void ParseRelearn(string[] queryArray)
        {
            Action = queryArray[0];
            Name = queryArray[1];
        }

        private void ParseClone(string[] queryArray)
        {
            Action = queryArray[0];
            Name = queryArray[1];
        }
    }

    public class Clone
    {
        public ImmutableStack<string> StudiedPrograms { get; set; }
        public string Name { get; set; }
        public ImmutableStack<string> RollbackHistory { get; set; }
        private readonly CloneVersionSystem _system;

        public Clone(CloneVersionSystem system, string name)
        {
            _system = system;
            RollbackHistory = ImmutableStack<string>.Empty;
            Name = name;
            StudiedPrograms = ImmutableStack<string>.Empty.Push("basic");
            _system.AllClones[Name] = this;
        }

        public Clone(CloneVersionSystem system, Clone originalClone)
        {
            _system = system;
            Name = (_system.AllClones.Count + 1).ToString();
            RollbackHistory = originalClone.RollbackHistory;
            StudiedPrograms = originalClone.StudiedPrograms;
            _system.AllClones[Name] = this;
        }
    }
}