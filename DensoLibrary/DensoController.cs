using System;
using System.Collections.Generic;
using BaseLibrary;
using ORiN2.interop.CAO;

namespace DensoLibrary
{
    public class DensoController : LogEventObject
    {
        public event Action<CaoMessage> MessageEvent;

        private readonly CaoController controller;

        public static string[] ControllerVarStrings =
        {
            //RC8
            "@MODE",
            "@LOCK",
            "@TIME",
            "@CURRENT_TIME",
            "@BUSY_STATUS",
            "@TSR_BUSY_STATUS",
            "@NORMAL_STATUS",
            "@ERROR_CODE",
            "@ERROR_CODE_HEX",
            "@ERROR_DESCRIPTION",
            "@EMERGENCY_STOP",
            "@DEADMAN_SW",
            "@AUTO_ENABLE",
            "@TYPE",
            "@SERIAL_NO",
            "@PROTECTIVE_STOP"


            //"@CURRENT_TIME",
            //"@FREE_USER_MEM",
            //"@NORMAL_STATUS",
            //"@AUTO_MODE",
            //"@MODE",
            //"@BUSY_STATUS",
            //"@EMERGENCY_STOP",
            //"@ERROR_CODE",
            //"@ERROR_CODE_HEX",
            //"@ERROR_DESCRIPTION",
            //"@MAKER_NAME",
            //"@TYPE",
            //"@VERSION",
            //"@SERIAL_NO",
        };


        public Dictionary<string, CaoVariable> ControllerCaoVars = new Dictionary<string, CaoVariable>();
        public Dictionary<string, CaoVariable> ControllerPointsPVars = new Dictionary<string, CaoVariable>();
        public Dictionary<string, CaoVariable> ControllerPointsJVars = new Dictionary<string, CaoVariable>();

        public DensoController(CaoController c)
        {
            controller = c;
            c.OnMessage += OnMessageEvent;

            OnLogEvent("Controller: robot add variable...");
            foreach (var s in ControllerVarStrings)
            {
                ControllerCaoVars.Add(s, controller.AddVariable(s, null));
            }

            for (int i = 0; i < 100; i++)
            {
                ControllerPointsPVars.Add("P" + i, controller.AddVariable("P" + i, null));
            }

            for (int i = 0; i < 100; i++)
            {
                ControllerPointsJVars.Add("J" + i, controller.AddVariable("J" + i, null));
            }
        }

        static List<string> str = new List<string>();

        public List<string> GetStatus()
        {
            str.Clear();

            foreach (var caoVar in ControllerCaoVars)
            {
                //str.Add(caoVar.Key + ":" + caoVar.Value.Value.ToString());
                str.Add(caoVar.Value.Value.ToString());
            }

            return str;
        }

        #region internal methods

        public void Execute(string cmd, params object[] paras)
        {
            controller.Execute(cmd, paras);
            OnLogEvent("Controller: Execute " + cmd);
        }


        protected virtual void OnMessageEvent(CaoMessage msg)
        {
            var handler = MessageEvent;
            if (handler != null) handler(msg);
        }

        #endregion

        #region external methods

        public int ErrorCode
        {
            get { return (int) ControllerCaoVars["@ERROR_CODE"].Value; }
        }

        public void Initialize()
        {
            TempPosVar99 = ControllerPointsPVars["P99"];

            HomeJointVar = ControllerPointsJVars["J10"];
            HomeJointVar.Value = new[] {0, 0, 90, 0, 0, 0};

            ObjectPosVar = ControllerPointsPVars["P11"];
            ObjectPosVar.Value = new[] {0, 0, 0, 0, 0, 0, -1};

            ObjectAngVar = ControllerPointsJVars["J11"];
            ObjectAngVar.Value = new[] {0, 0, 0, 0, 0, 0};
        }


        public string GetAutoMode()
        {
            switch ((int) controller.Execute("GetAutoMode", null))
            {
                case 0:
                    return "unknown";
                case 1:
                    return "internal";
                case 2:
                    return "external";
                default:
                    return "error";
            }
        }

        public void PutAutoMode()
        {
            Execute("PutAutoMode", 2);
            OnLogEvent("Controller: PutAutoMode-External");
        }

        public void SetExtension(int mode, int key)
        {
            Execute("SetExtension", new[] {mode, key});
            OnLogEvent(string.Format("Controller: SetExtension {0} {1}", mode, key));
        }


        public void ClearError()
        {
            int e = (int) ControllerCaoVars["@ERROR_CODE"].Value;
            if (e != 0)
            {
                Execute("ClearError", e);
                OnLogEvent(string.Format("Controller: ClearError {0} {1}", e,
                    ControllerCaoVars["@ERROR_DESCRIPTION"].Value.ToString()));
            }
        }

        public CaoVariable HomeJointVar;
        public CaoVariable ObjectPosVar;
        public CaoVariable ObjectAngVar;
        public CaoVariable TempPosVar99;


        public void SetObjectPos(CaoVariable pos, double offsetX = 0, double offsetY = 0, double offsetZ = 0)
        {
            double[] posData = (double[]) pos.Value;
            posData[0] += offsetX;
            posData[1] += offsetY;
            posData[2] += offsetZ;

            ObjectPosVar.Value = posData;
        }


        public void SetObjectAng(CaoVariable pos, int axis = 1, double offsetA = 0)
        {
            double[] posData = (double[]) pos.Value;
            posData[axis - 1] += offsetA;

            ObjectAngVar.Value = posData;
        }

        #endregion
    }
}