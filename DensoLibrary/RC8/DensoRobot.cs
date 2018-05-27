using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BaseLibrary.Object;
using ORiN2.interop.CAO;

namespace DensoLibrary.RC8
{
    public class DensoRobot : LogEventObject
    {
        private readonly CaoRobot robot;

        public DensoRobot(CaoRobot r)
        {
            robot = r;

            OnLogEvent("Robot: robot add variable...");

            foreach (var s in RobotVarStrings)
            {
                RobCaoVars.Add(s, robot.AddVariable(s, ""));
            }
        }

        #region status

        public static string[] RobotVarStrings =
        {
            //RC8
            "@CURRENT_POSITION",
            "@CURRENT_ANGLE",
            "@SERVO_ON",
            "@BUSY_STATUS",
            "@TYPE_NAME",
            "@TYPE",
            "@CURRENT_TRANS",
            "@CURRENT_TOOL",
            "@CURRENT_WORK",
            "@SPEED",
            "@ACCEL",
            "@DECEL",
            "@JSPEED",
            "@JACCEL",
            "@JDECEL",
            "@EXTSPEED",
            "@EXTACCEL",
            "@EXTDECEL",
            "@HIGH_CURRENT_POSITION",
            "@HIGH_CURRENT_ANGLE",
            "@HIGH_CURRENT_TRANS"


            //RC7
            //"@CURRENT_POSITION",
            //"@CURRENT_ANGLE",
            //"@SERVO_ON",
            //"@ZERO_RETURN_REQUIRED",
            //"@BUSY_STATUS",
            //"@TYPE_NAME",
            //"@TYPE",
            //"@CURRENT_TRANS",
            //"@CURRENT_TOOL",
            //"@CURRENT_WORK",
            //"@SPEED",
            //"@ACCEL",
            //"@DECEL",
            //"@JSPEED",
            //"@JACCEL",
            //"@JDECEL",
            //"@EXTSPEED",
            //"@EXTACCEL",
            //"@EXTDECEL",
            //"@HIGH_CURRENT_POSITION",
            //"@HIGH_CURRENT_ANGLE",
            //"@HIGH_CURRENT_TRANS",
        };

        public Dictionary<string, CaoVariable> RobCaoVars = new Dictionary<string, CaoVariable>();
        private static readonly List<string> status = new List<string>();

        public List<string> GetStatus()
        {
            status.Clear();
            foreach (var caoVar in RobCaoVars)
            {
                if (caoVar.Key == "@CURRENT_POSITION" ||
                    caoVar.Key == "@CURRENT_ANGLE" ||
                    caoVar.Key == "@CURRENT_TRANS"
                    )
                {
                    var pos = (double[]) caoVar.Value.Value;

                    var sb = new StringBuilder();
                    if (caoVar.Key == "@CURRENT_POSITION")
                    {
                        sb.Append("P(");
                    }
                    if (caoVar.Key == "@CURRENT_ANGLE")
                    {
                        sb.Append("J(");
                    }
                    if (caoVar.Key == "@CURRENT_TRANS")
                    {
                        sb.Append("T(");
                    }

                    for (var i = 0; i < pos.Length; i++)
                    {
                        sb.Append(pos[i].ToString("0.00") + ",");
                    }
                    sb.Append(")");

                    status.Add(sb.ToString());
                    continue;
                }

                if (caoVar.Key == "@HIGH_CURRENT_POSITION" ||
                    caoVar.Key == "@HIGH_CURRENT_ANGLE" ||
                    caoVar.Key == "@HIGH_CURRENT_TRANS"
                    )
                {
                    //float[] pos = (float[])caoVar.Value.Value;

                    //StringBuilder sb = new StringBuilder();
                    //if (caoVar.Key == "@HIGH_CURRENT_POSITION")
                    //{
                    //    sb.Append("P(");
                    //}
                    //if (caoVar.Key == "@HIGH_CURRENT_ANGLE")
                    //{
                    //    sb.Append("J(");
                    //}
                    //if (caoVar.Key == "@HIGH_CURRENT_TRANS")
                    //{
                    //    sb.Append("T(");
                    //}

                    //for (int d = 0; d < pos.Length; d++)
                    //{
                    //    sb.Append(pos[d].ToString("0.00") + ",");
                    //}
                    //sb.Append(")");

                    //status.Add(sb.ToString());
                    continue;
                }

                //status.Add(caoVar.Key + ":" + caoVar.Value.Value.ToString());
                status.Add(caoVar.Value.Value.ToString());
            }

            return status;
        }

        #endregion

        #region object move

        public void MoveObjectPos(int mode)
        {
            try
            {
                robot.Move(mode, "@E P11", "NEXT");
                //OnLogEvent("Robot: MoveP @E P11.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void MoveObjectPos(int mode, int time)
        {
            try
            {
                robot.Move(mode, "@E P11", string.Format("TIME={0:D},NEXT", time));
                //OnLogEvent("Robot: MoveP @E P11.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void MoveObjectAng(int mode)
        {
            try
            {
                robot.Move(mode, "@E J11", "NEXT");
                //OnLogEvent("Robot: MoveP @E J11.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///     try get temp pos P99 to joint
        /// </summary>
        /// <returns></returns>
        public double[] GetP2J()
        {
            return robot.Execute("P2J", "P99") as double[];
        }

        #endregion

        #region internal methods

        public void Halt()
        {
            robot.Halt("");
            OnLogEvent("Robot: Halt");
        }

        public void ChangeTool(int index)
        {
            robot.Change("Tool" + index);
            OnLogEvent("Robot: Change Tool" + index);
        }

        public void Drive()
        {
            throw new NotImplementedException();
        }


        public void MoveP(int mode, int pIndex)
        {
            OnLogEvent(string.Format("Robot: Move {0} @E P{1}", mode, pIndex));
            robot.Move(mode, "@E P" + pIndex, "NEXT");
        }

        public void MoveJ(int mode, int pIndex)
        {
            OnLogEvent(string.Format("Robot: Move {0} @E J{1}", mode, pIndex));
            robot.Move(mode, "@E J" + pIndex, "NEXT");
        }

        public void MoveC(int passIndex, int objectIndex)
        {
            robot.Move(3, "P" + passIndex + ",@E P" + objectIndex, "NEXT");
            OnLogEvent(string.Format("Robot: Move {0} P{1} @E P{2}", 3, passIndex, objectIndex));
        }

        public void MoveS(int path)
        {
            robot.Move(4, path, "NEXT");
            OnLogEvent(string.Format("Robot: Move {0} {1},NEXT", 4, path));
        }

        /// <summary>
        ///     如果指定“Pose = 1”，则姿势随着旋转变化。
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="degree"></param>
        /// <param name="vector"></param>
        public void Rotate1(int mode, float degree, string vector)
        {
            switch (mode)
            {
                case 1:
                    robot.Rotate("YZ", degree, vector, "@0,Pose = 1,NEXT");
                    OnLogEvent(string.Format("Robot: Rotate1 YZ {0} {1}", degree, vector));
                    break;
                case 2:
                    robot.Rotate("ZX", degree, vector, "@0,Pose = 1,NEXT");
                    OnLogEvent(string.Format("Robot: Rotate1 ZX {0} {1}", degree, vector));
                    break;
                case 3:
                    robot.Rotate("XY", degree, vector, "@0,Pose = 1,NEXT");
                    OnLogEvent(string.Format("Robot: Rotate1 XY {0} {1}", degree, vector));
                    break;
            }
        }

        /// <summary>
        ///     如果指定“Pose = 2”，则仅保持当前位置（起始点）的姿势，沿轨迹执行旋转动作。
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="degree"></param>
        /// <param name="vector"></param>
        public void Rotate2(int mode, float degree, string vector)
        {
            switch (mode)
            {
                case 1:
                    robot.Rotate("YZ", degree, vector, "@0,Pose = 2,NEXT");
                    OnLogEvent(string.Format("Robot: Rotate1 YZ {0} {1}", degree, vector));
                    break;
                case 2:
                    robot.Rotate("ZX", degree, vector, "@0,Pose = 2,NEXT");
                    OnLogEvent(string.Format("Robot: Rotate1 ZX {0} {1}", degree, vector));
                    break;
                case 3:
                    robot.Rotate("XY", degree, vector, "@0,Pose = 2,NEXT");
                    OnLogEvent(string.Format("Robot: Rotate1 XY {0} {1}", degree, vector));
                    break;
            }
        }

        public void Rotate2(int mode, float degree, string vector, int time)
        {
            switch (mode)
            {
                case 1:
                    robot.Rotate("YZ", degree, vector, string.Format("@0,Pose = 2,TIME = {0},NEXT", time));
                    OnLogEvent(string.Format("Robot: Rotate1 YZ {0} {1}", degree, vector));
                    break;
                case 2:
                    robot.Rotate("ZX", degree, vector, string.Format("@0,Pose = 2,TIME = {0},NEXT", time));
                    OnLogEvent(string.Format("Robot: Rotate1 ZX {0} {1}", degree, vector));
                    break;
                case 3:
                    robot.Rotate("XY", degree, vector, string.Format("@0,Pose = 2,TIME = {0},NEXT", time));
                    OnLogEvent(string.Format("Robot: Rotate1 XY {0} {1}", degree, vector));
                    break;
            }
        }

        public void Speed(int axis, float speed)
        {
            robot.Speed(axis, speed);
            OnLogEvent(string.Format("Robot: Speed Axis {0} Speed {1} ", axis, speed));
        }

        public void Execute(string cmd, params object[] paras)
        {
            robot.Execute(cmd, paras);

            var sb = new StringBuilder();
            foreach (var para in paras)
            {
                sb.Append(para + " ");
            }
            OnLogEvent(string.Format("Robot: Execute {0} {1}", cmd, sb));
        }

        #endregion

        #region external methods

        public bool BusyStatus
        {
            get { return (bool) RobCaoVars["@BUSY_STATUS"].Value; }
        }

        public CaoVariable CurPosVar
        {
            get
            {
                if (RobCaoVars != null)
                {
                    return RobCaoVars["@CURRENT_POSITION"];
                }
                throw new NotImplementedException();
            }
        }

        public CaoVariable CurTransVar
        {
            get
            {
                if (RobCaoVars != null)
                {
                    return RobCaoVars["@CURRENT_TRANS"];
                }
                throw new NotImplementedException();
            }
        }

        public CaoVariable CurAngVar
        {
            get
            {
                if (RobCaoVars != null)
                {
                    return RobCaoVars["@CURRENT_ANGLE"];
                }
                throw new NotImplementedException();
            }
        }

        public void Home()
        {
            if (robot == null)
            {
                MessageBox.Show("robot not set!");
                return;
            }

            OnLogEvent("Robot: MoveP J(0,0,0,0,90,0)");
            robot.Move(1, "@E J10", null);
        }

        public void Motor(int sts)
        {
            Execute("Motor", sts);
        }

        public void ExtSpeed(int s)
        {
            Execute("ExtSpeed", s, s, s);
        }

        public void DefTool(int index, float[] p)
        {
            Execute("SetToolDef", index, DensoControl.GetPString(p));
        }


        public void LoadPathPoint(int path)
        {
            Execute("LoadPathPoint", path);
        }

        public void AddPathPoint(int path, int point)
        {
            Execute("AddPathPoint", path.ToString(), "P" + point);
        }

        public void ClrPathPoint(int path)
        {
            Execute("ClrPathPoint", path);
        }

        #endregion
    }
}