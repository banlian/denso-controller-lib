using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BaseLibrary.Object;
using ORiN2.interop.CAO;

namespace DensoLibrary.RC8
{
    public class DensoControl : LogEventObject
    {
        public void Kill()
        {
            var processes = Process.GetProcessesByName("CAO");
            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        #region singleton

        private static DensoControl ins;

        private DensoControl()
        {
        }

        public static DensoControl Only()
        {
            return ins ?? (ins = new DensoControl());
        }

        #endregion

        #region init vars

        private CaoEngine caoEngine;
        private CaoWorkspace caoWorkspace;
        private CaoController caoController;
        private CaoRobot caoRobot;

        public DensoController Controller { get; private set; }
        public DensoRobot Robot { get; private set; }
        public bool IsInitialized { get; private set; }

        #endregion

        #region status strings

        public List<string> ControllerStatus
        {
            get { return Controller.GetStatus(); }
        }

        public List<string> RobotStatus
        {
            get { return Robot.GetStatus(); }
        }

        #endregion

        #region init methods

        public void Init()
        {
            if (IsInitialized)
            {
                return;
            }

            Kill();

            // Create CaoEngine object
            caoEngine = new CaoEngine();
            caoWorkspace = caoEngine.Workspaces.Item(0);
            //caoController = caoWorkspace.AddController("RC7", "CaoProv.DENSO.NetwoRC", "", "conn=eth:192.168.0.1");
            caoController = caoWorkspace.AddController("RC8", "CaoProv.DENSO.RC8", null,
                "Server=192.168.0.1,@IfNotMember=True");
            caoRobot = caoController.AddRobot("Arm", "");


            Robot = new DensoRobot(caoRobot);
            Robot.LogEvent += OnLogEvent;


            Controller = new DensoController(caoController);
            Controller.LogEvent += OnLogEvent;
            Controller.ClearError();
            if ((int) Controller.ControllerCaoVars["@MODE"].Value <= 2)
            {
                MessageBox.Show("Controller not in Auto Mode! Retry!");
                Close();
                Kill();
                IsInitialized = false;
                return;
            }
            //Controller.PutAutoMode();
            Controller.Initialize();


            //RobslaveTask = new DensoTask(caoController.AddTask("robslave", null));
            //RobslaveTask.LogEvent += OnLogEvent;
            //RobslaveTask.Stop();
            //RobslaveTask.Start();

            IsInitialized = true;
        }

        public void TakeArm()
        {
            try
            {
                if (Robot == null)
                {
                    MessageBox.Show("Robot Disposed!");
                    return;
                }
                Robot.Motor(1);
                Robot.Execute("Takearm", 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void GiveArm()
        {
            try
            {
                if (Robot == null)
                {
                    MessageBox.Show("Robot Disposed!");
                    return;
                }

                //densoRobot.Halt();
                Robot.Execute("Givearm", "");
                Robot.Execute("Motor", 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        public void Close()
        {
            IsInitialized = false;

            // Release caoRobot object
            if (caoRobot != null)
            {
                caoRobot.Variables.Clear();
                Marshal.ReleaseComObject(caoRobot);
                caoRobot = null;
            }

            // Release controller object
            if (caoController != null)
            {
                caoController.Variables.Clear();
                caoController.Robots.Clear();
                Marshal.ReleaseComObject(caoController);
                caoController = null;
            }


            if (caoWorkspace != null)
            {
                caoWorkspace.Controllers.Clear();
                Marshal.ReleaseComObject(caoWorkspace);
                caoWorkspace = null;
            }

            if (caoEngine != null)
            {
                caoEngine.Workspaces.Clear();
                Marshal.ReleaseComObject(caoEngine);
                caoEngine = null;
            }
        }

        public void Reset()
        {
            //Close();
            //Initialize();

            MessageBox.Show(string.Format("Error {0} {1}",
                Controller.ControllerCaoVars["@ERROR_CODE"].Value,
                Controller.ControllerCaoVars["@ERROR_DESCRIPTION"].Value));

            Controller.ClearError();
            TakeArm();
        }

        #endregion

        #region motion methods

        public void GoHome()
        {
            try
            {
                Robot.Home();
            }
            catch (Exception)
            {
                OnLogEvent("Robot: Home Error");
                Reset();
            }
        }

        public void MoveObjectPos(int mode, float x, float y, float z)
        {
            Controller.SetObjectPos(Robot.CurPosVar, x, y, z);
            MoveObjectPos(mode);
        }

        public void MoveObjectPos(int mode, int axis, float offset)
        {
            Controller.SetObjectPos(Robot.CurPosVar, axis, offset);
            MoveObjectPos(mode);
        }

        public void MoveObjectPos(int mode)
        {
            try
            {
                OnLogEvent("Robot: MoveObjectPos " + GetPString(Controller.ObjectPosVar));
                Robot.MoveObjectPos(mode);
            }
            catch (Exception)
            {
                OnLogEvent("Robot: MoveObjectPos Error");
                Reset();
            }
        }


        public void MoveObjectAng(int mode, int axis, float offset)
        {
            Controller.SetObjectAng(Robot.CurAngVar, axis, offset);
            MoveObjectAng(mode);
        }


        public bool MoveObjectAng(int mode)
        {
            try
            {
                OnLogEvent("Robot: MoveObjectAng " + GetJString(Controller.ObjectAngVar));
                Robot.MoveObjectAng(mode);
            }
            catch (Exception)
            {
                OnLogEvent("Robot: MoveObjectAng Error");
                Reset();
                return false;
            }

            return true;
        }


        public void Rotate1(int axis, float degree)
        {
            try
            {
                var cur = (double[]) Robot.CurPosVar.Value;

                var x = cur[0];
                var y = cur[1];
                var z = cur[2];

                Robot.Rotate1(axis, degree, string.Format("V({0:F},{1:F},{2:F})", x, y, z));
            }
            catch (Exception)
            {
                OnLogEvent("Robot: Rotate1 Error");
                Reset();
            }
        }

        public void Rotate1(int axis, float degree, double x, double y, double z)
        {
            try
            {
                Robot.Rotate1(axis, degree, string.Format("V({0:F},{1:F},{2:F})", x, y, z));
            }
            catch (Exception)
            {
                OnLogEvent("Robot: Rotate1 Error");
                Reset();
            }
        }


        public void Rotate2(int axis, float degree)
        {
            try
            {
                var cur = (double[]) Robot.CurPosVar.Value;

                var x = cur[0];
                var y = cur[1];
                var z = cur[2];

                Robot.Rotate2(axis, degree, string.Format("V({0:F},{1:F},{2:F})", x, y, z));
            }
            catch (Exception)
            {
                OnLogEvent("Robot: Rotate2 Error");
                Reset();
            }
        }

        public void Rotate2(int axis, float degree, double x, double y, double z)
        {
            try
            {
                Robot.Rotate2(axis, degree, string.Format("V({0:F},{1:F},{2:F})", x, y, z));
            }
            catch (Exception)
            {
                OnLogEvent("Robot: Rotate2 Error");
                Reset();
            }
        }


        public void Rotate2(int axis, float degree, double x, double y, double z, int time)
        {
            try
            {
                Robot.Rotate2(axis, degree, string.Format("V({0:F},{1:F},{2:F})", x, y, z), time);
            }
            catch (Exception)
            {
                OnLogEvent("Robot: Rotate2 Error");
                Reset();
            }
        }

        #endregion

        #region static string methods

        public static string GetPosData(CaoVariable pVar)
        {
            var doubleVals = pVar.Value as double[];
            if (doubleVals != null)
            {
                var pos = doubleVals;

                var sb = new StringBuilder();
                for (var i = 0; i < 3; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }

            var floatVals = pVar.Value as float[];
            if (floatVals != null)
            {
                var pos = floatVals;
                var sb = new StringBuilder();
                for (var i = 0; i < 3; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }

            return "Error";
        }

        public static string GetPString(CaoVariable pVar)
        {
            var doubleVals = pVar.Value as double[];
            if (doubleVals != null)
            {
                var pos = doubleVals;

                var sb = new StringBuilder();
                sb.Append("P(");
                for (var i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");

                return sb.ToString();
            }
            var floatVals = pVar.Value as float[];
            if (floatVals != null)
            {
                var pos = floatVals;
                var sb = new StringBuilder();
                sb.Append("P(");
                for (var i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");

                return sb.ToString();
            }

            return "Error";
        }

        public static string GetPString(float[] pos)
        {
            var sb = new StringBuilder();
            sb.Append("P(");
            for (var i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetPString(double[] pos)
        {
            var sb = new StringBuilder();
            sb.Append("P(");
            for (var i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            return sb.ToString();
        }


        public static string GetJString(CaoVariable pVar)
        {
            if (pVar.Value is double[])
            {
                var pos = (double[]) pVar.Value;

                var sb = new StringBuilder();
                sb.Append("J(");
                for (var i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");

                return sb.ToString();
            }
            if (pVar.Value is float[])
            {
                var pos = (float[]) pVar.Value;
                var sb = new StringBuilder();
                sb.Append("J(");
                for (var i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");

                return sb.ToString();
            }

            return "Error";
        }

        public static string GetJString(float[] pos)
        {
            var sb = new StringBuilder();
            sb.Append("J(");
            for (var i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetJString(double[] pos)
        {
            var sb = new StringBuilder();
            sb.Append("J(");
            for (var i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetTString(CaoVariable pVar)
        {
            var doubleVals = pVar.Value as double[];
            if (doubleVals != null)
            {
                var pos = doubleVals;

                var sb = new StringBuilder();
                sb.Append("T(");
                for (var i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                return sb.ToString();
            }

            var floatVals = pVar.Value as float[];
            if (floatVals != null)
            {
                var pos = floatVals;
                var sb = new StringBuilder();
                sb.Append("T(");
                for (var i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                return sb.ToString();
            }

            return "Error";
        }

        #endregion
    }
}