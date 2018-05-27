using System.Collections.Generic;
using BaseLibrary;
using ORiN2.interop.CAO;

namespace DensoLibrary
{
    /// <summary>
    /// using for RC7 Task Control
    /// </summary>
    public class DensoTask : LogEventObject
    {
        private CaoTask task;

        public static string[] TaskVarStrings =
        {
            "@STATUS",
            "@PRIORITY",
            "@LINE_NO",
            "@CYCLE_TIME",
            //"@START",
            //"@STOP",
        };

        public Dictionary<string, CaoVariable> TaskCaoVars = new Dictionary<string, CaoVariable>();

        public DensoTask(CaoTask t)
        {
            task = t;

            foreach (var s in TaskVarStrings)
            {
                TaskCaoVars.Add(s, task.AddVariable(s, ""));
            }
        }

        static List<string> str = new List<string>();

        public List<string> GetStatus()
        {
            str.Clear();

            foreach (var caoVar in TaskCaoVars)
            {
                //str.Add(caoVar.Key + ":" + caoVar.Value.Value.ToString());
                str.Add(caoVar.Value.Value.ToString());
            }

            return str;
        }


        public void Start()
        {
            task.Start(1, null);
        }


        public void Stop()
        {
            task.Stop(4, null);
        }

        public void Execute(string cmd, params object[] paras)
        {
            task.Execute(cmd, paras);
            OnLogEvent("Task: Execute-" + cmd);
        }
    }
}