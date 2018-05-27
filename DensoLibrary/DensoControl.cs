using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using ORiN2.interop.CAO;

namespace DensoLibrary
{
    public class DensoControl : LogEventObject
    {
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
        public DensoTask RobslaveTask { get; private set; }
        public DensoRobot Robot { get; private set; }

        public bool IsInitialized { get; private set; }

        #endregion

        #region status strings

        public List<string> ControllerStatus
        {
            get { return Controller.GetStatus(); }
        }

        public List<string> RobslaveTaskStatus
        {
            get { return RobslaveTask.GetStatus(); }
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

            IsInitialized = false;
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
                Reset();
            }
        }

        public void MoveObjectPos(int mode)
        {
            try
            {
                Logger.Line("Robot: MoveObjectPos " + GetPString(Controller.ObjectPosVar));
                Robot.MoveObjectPos(mode);

                while (Robot.BusyStatus)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                Logger.Line("Robot: MoveObjectPos Error");
                Reset();
            }
        }


        public void MoveObjectPos(int mode, int axis, float offset)
        {
            switch (axis)
            {
                case 1:
                    Controller.SetObjectPos(Robot.CurPosVar, offset);
                    break;
                case 2:
                    Controller.SetObjectPos(Robot.CurPosVar, 0, offset);
                    break;
                case 3:
                    Controller.SetObjectPos(Robot.CurPosVar, 0, 0, offset);
                    break;
            }
            MoveObjectPos(mode);
        }

        public bool MoveObjectAng(int mode)
        {
            try
            {
                Logger.Line("Robot: MoveObjectAng " + GetJString(Controller.ObjectAngVar));
                Robot.MoveObjectAng(mode);

                while (Robot.BusyStatus)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                Logger.Line("Robot: MoveObjectAng Error");
                Reset();
                return false;
            }

            return true;
        }


        public void MoveObjectAng(int mode, int axis, float offset)
        {
            Controller.SetObjectAng(Robot.CurAngVar, axis, offset);
            MoveObjectAng(mode);
        }


        public void Rotate(int mode, float degree)
        {
            CaoVariable cur = Robot.CurPosVar;

            double x = cur.Value[0];
            double y = cur.Value[1];
            double z = cur.Value[2];

            Robot.Rotate(mode, degree, string.Format("V({0},{1},{2})", x, y, z));
        }

        public void Rotate(int mode, float degree, double x, double y, double z)
        {
            Robot.Rotate(mode, degree, string.Format("V({0},{1},{2})", x, y, z));
        }

        #endregion

        #region static string methods

        public static string GetPString(CaoVariable pVar)
        {
            if (pVar.Value is double[])
            {
                double[] pos = (double[]) pVar.Value;

                StringBuilder sb = new StringBuilder();
                sb.Append("P(");
                for (int i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Append(")");

                return sb.ToString();
            }
            else if (pVar.Value is float[])
            {
                float[] pos = (float[]) pVar.Value;
                StringBuilder sb = new StringBuilder();
                sb.Append("P(");
                for (int i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Append(")");

                return sb.ToString();
            }

            return "Error";
        }

        public static string GetPString(float[] pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("P(");
            for (int i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetPString(double[] pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("P(");
            for (int i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Append(")");

            return sb.ToString();
        }


        public static string GetJString(CaoVariable pVar)
        {
            if (pVar.Value is double[])
            {
                double[] pos = (double[]) pVar.Value;

                StringBuilder sb = new StringBuilder();
                sb.Append("J(");
                for (int i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Append(")");

                return sb.ToString();
            }
            else if (pVar.Value is float[])
            {
                float[] pos = (float[]) pVar.Value;
                StringBuilder sb = new StringBuilder();
                sb.Append("J(");
                for (int i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Append(")");

                return sb.ToString();
            }

            return "Error";
        }

        public static string GetJString(float[] pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("J(");
            for (int i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetJString(double[] pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("J(");
            for (int i = 0; i < pos.Length; i++)
            {
                sb.Append(pos[i].ToString("0.00") + ",");
            }
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetTString(CaoVariable pVar)
        {
            if (pVar.Value is double[])
            {
                double[] pos = (double[]) pVar.Value;

                StringBuilder sb = new StringBuilder();
                sb.Append("T(");
                for (int i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Append(")");

                return sb.ToString();
            }
            else if (pVar.Value is float[])
            {
                float[] pos = (float[]) pVar.Value;
                StringBuilder sb = new StringBuilder();
                sb.Append("T(");
                for (int i = 0; i < pos.Length; i++)
                {
                    sb.Append(pos[i].ToString("0.00") + ",");
                }
                sb.Append(")");

                return sb.ToString();
            }

            return "Error";
        }

        #endregion

        public void Kill()
        {
            Process[] processes = System.Diagnostics.Process.GetProcessesByName("CAO");
            foreach (var process in processes)
            {
                process.Kill();
            }
        }
    }
}