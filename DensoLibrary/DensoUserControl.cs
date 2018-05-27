using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DensoLibrary.RC8;

namespace DensoLibrary
{
    public partial class DensoUserControl : UserControl
    {
        private bool isXyzMode;

        private int timerIndex;

        public DensoUserControl()
        {
            InitializeComponent();
        }


        private void DensoUserControl_Load(object sender, EventArgs e)
        {
            isXyzMode = true;
            radioButtonXYZMode.Checked = true;
            radioButtonJointMode.Checked = false;


            comboBoxPointIndex.Items.Add("-1");
            for (var i = 10; i < 200; i++)
            {
                comboBoxPointIndex.Items.Add("P" + i);
            }
            comboBoxPointIndex.SelectedIndex = 0;


            comboBoxJointIndex.Items.Add("-1");
            for (var i = 10; i < 100; i++)
            {
                comboBoxJointIndex.Items.Add("J" + i);
            }
            comboBoxJointIndex.SelectedIndex = 0;


            //controller
            dataGridViewController.Columns.Add("Description", "Description");
            dataGridViewController.Columns.Add("Status", "Status");
            dataGridViewController.Columns[1].Width = 400;
            for (var i = 0; i < dataGridViewController.Columns.Count; i++)
            {
                dataGridViewController.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //robot
            dataGridViewRobot.Columns.Add("Description", "Description");
            dataGridViewRobot.Columns.Add("Status", "Status");
            dataGridViewRobot.Columns[1].Width = 400;
            for (var i = 0; i < dataGridViewRobot.Columns.Count; i++)
            {
                dataGridViewRobot.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }


            //Task
            dataGridViewTask.Columns.Add("Description", "Description");
            dataGridViewTask.Columns.Add("Status", "Status");
            dataGridViewTask.Columns[1].Width = 400;
            for (var i = 0; i < dataGridViewTask.Columns.Count; i++)
            {
                dataGridViewTask.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //P
            dataGridViewPointsP.Columns.Add("Index", "Index");
            dataGridViewPointsP.Columns.Add("Details", "Details");
            dataGridViewPointsP.Columns[1].Width = 400;
            for (var i = 0; i < dataGridViewPointsP.Columns.Count; i++)
            {
                dataGridViewPointsP.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //J
            dataGridViewPointsJ.Columns.Add("Index", "Index");
            dataGridViewPointsJ.Columns.Add("Details", "Details");
            dataGridViewPointsJ.Columns[1].Width = 400;
            for (var i = 0; i < dataGridViewPointsJ.Columns.Count; i++)
            {
                dataGridViewPointsJ.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            #region

            //controller status
            try
            {
                textBoxMode.Text = DensoControl.Only().Controller.ControllerCaoVars["@MODE"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxMode.Text = "?";
            }

            try
            {
                textBoxAutoMode.Text = DensoControl.Only().Controller.ControllerCaoVars["@AUTO_ENABLE"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxAutoMode.Text = "?";
            }

            try
            {
                textBoxEmgency.Text =
                    DensoControl.Only().Controller.ControllerCaoVars["@EMERGENCY_STOP"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxEmgency.Text = "?";
            }


            try
            {
                textBoxNormalStatus.Text =
                    DensoControl.Only().Controller.ControllerCaoVars["@NORMAL_STATUS"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxNormalStatus.Text = "?";
            }


            try
            {
                textBoxErrorCode.Text = DensoControl.Only().Controller.ControllerCaoVars["@ERROR_CODE"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxErrorCode.Text = "?";
            }


            try
            {
                textBoxErrorCodeInfo.Text =
                    DensoControl.Only().Controller.ControllerCaoVars["@ERROR_DESCRIPTION"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxErrorCodeInfo.Text = "?";
            }


            ////robot slave task
            //try
            //{
            //    textBoxRobotSlaveTask.Text = DensoControl.Only().RobslaveTask.TaskCaoVars["@STATUS"].Value.ToString();
            //}
            //catch (Exception)
            //{
            //    textBoxRobotSlaveTask.Text = "?";
            //}


            //robot status
            try
            {
                textBoxMotorStatus.Text = DensoControl.Only().Robot.RobCaoVars["@SERVO_ON"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxMotorStatus.Text = "???";
            }

            try
            {
                textBoxBusyStatus.Text = DensoControl.Only().Robot.RobCaoVars["@BUSY_STATUS"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxBusyStatus.Text = "???";
            }

            try
            {
                // Current Position
                textBoxCurPos.Text = DensoControl.GetPString(DensoControl.Only().Robot.RobCaoVars["@CURRENT_POSITION"]);
            }
            catch (Exception)
            {
                textBoxCurPos.Text = "P(?,?,?,?,?,?,?)";
            }


            try
            {
                // Current Position
                textBoxCurAngle.Text = DensoControl.GetJString(DensoControl.Only().Robot.RobCaoVars["@CURRENT_ANGLE"]);
            }
            catch (Exception)
            {
                textBoxCurAngle.Text = "J(?,?,?,?,?,?,?)";
            }

            try
            {
                // Current Position
                textBoxCurTrans.Text = DensoControl.GetTString(DensoControl.Only().Robot.RobCaoVars["@CURRENT_TRANS"]);
            }
            catch (Exception)
            {
                textBoxCurTrans.Text = "T(?,?,?,?,?,?,?,?,?,?)";
            }

            try
            {
                textBoxWorkStatus.Text = DensoControl.Only().Robot.RobCaoVars["@CURRENT_WORK"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxWorkStatus.Text = "???";
            }

            try
            {
                textBoxToolStatus.Text = DensoControl.Only().Robot.RobCaoVars["@CURRENT_TOOL"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxToolStatus.Text = "???";
            }

            try
            {
                textBoxSpeed.Text = DensoControl.Only().Robot.RobCaoVars["@SPEED"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxSpeed.Text = "???";
            }

            try
            {
                textBoxExtSpeed.Text = DensoControl.Only().Robot.RobCaoVars["@EXTSPEED"].Value.ToString();
            }
            catch (Exception)
            {
                textBoxExtSpeed.Text = "???";
            }

            #endregion

            var sts = DensoControl.Only().ControllerStatus;
            for (var i = 0; i < sts.Count; i++)
            {
                dataGridViewController.Rows[i].Cells[1].Value = sts[i];
            }

            var sts1 = DensoControl.Only().RobotStatus;
            for (var i = 0; i < sts1.Count; i++)
            {
                dataGridViewRobot.Rows[i].Cells[1].Value = sts1[i];
            }


            if (timerIndex++ > 5)
            {
                timerIndex = 0;

                for (var i = 0; i < DensoControl.Only().Controller.ControllerPointsPVars.Count; i++)
                {
                    var str = DensoControl.GetPString(DensoControl.Only().Controller.ControllerPointsPVars["P" + i]);
                    dataGridViewPointsP.Rows[i].Cells[1].Value = str;
                }

                for (var i = 0; i < DensoControl.Only().Controller.ControllerPointsJVars.Count; i++)
                {
                    var str = DensoControl.GetJString(DensoControl.Only().Controller.ControllerPointsJVars["J" + i]);
                    dataGridViewPointsJ.Rows[i].Cells[1].Value = str;
                }
            }


            //List<string> sts2 = DensoControl.Only().RobslaveTaskStatus;
            //for (int i = 0; i < sts2.Count; i++)
            //{
            //    dataGridViewTask.Rows[i].Cells[1].Value = sts2[i];
            //}
        }

        #region robot init

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            DensoControl.Only().Init();
            if (!DensoControl.Only().IsInitialized)
            {
                return;
            }


            //init ui

            //Controller
            dataGridViewController.Rows.Clear();
            foreach (var s in DensoController.ControllerVarStrings)
            {
                dataGridViewController.Rows.Add(s);
            }

            //P
            foreach (var p in DensoControl.Only().Controller.ControllerPointsPVars)
            {
                dataGridViewPointsP.Rows.Add(p.Key);
            }

            //J
            foreach (var p in DensoControl.Only().Controller.ControllerPointsJVars)
            {
                dataGridViewPointsJ.Rows.Add(p.Key);
            }


            //Robot
            dataGridViewRobot.Rows.Clear();
            foreach (var s in DensoRobot.RobotVarStrings)
            {
                dataGridViewRobot.Rows.Add(s);
            }


            ////Task
            //dataGridViewTask.Rows.Clear();
            //foreach (var s in DensoTask.TaskVarStrings)
            //{
            //    dataGridViewTask.Rows.Add(s);
            //}

            timerUpdate.Interval = 800;
            timerUpdate.Enabled = true;
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            timerUpdate.Enabled = false;

            DensoControl.Only().Close();
        }

        private void buttonTakeArm_Click(object sender, EventArgs e)
        {
            DensoControl.Only().TakeArm();
        }

        private void buttonGivearm_Click(object sender, EventArgs e)
        {
            DensoControl.Only().GiveArm();
        }

        private void buttonExtSpeed_Click(object sender, EventArgs e)
        {
            int speed;
            if (int.TryParse(textBoxSetExtSpeed.Text, out speed)
                && speed > 0 && speed <= 90)
            {
                DensoControl.Only().Robot.ExtSpeed(speed);
            }
            else
            {
                MessageBox.Show("Speed Not In Range!");
            }
        }

        private void buttonChangeTool_Click(object sender, EventArgs e)
        {
            DensoControl.Only().Robot.ChangeTool(int.Parse(textBoxChangeToolIndex.Text));
        }

        private void buttonDefTool_Click(object sender, EventArgs e)
        {
            DensoControl.Only().Robot.DefTool(1, new[] {0, 0, 0, 0, 0, 0f});
        }

        #endregion

        #region robot move test

        private void buttonMoveP1_Click(object sender, EventArgs e)
        {
            int p;
            if (int.TryParse(textBoxP1.Text, out p)
                && p >= 10 && p < 200)
            {
                DensoControl.Only().Robot.MoveP(1, p);
            }
            else
            {
                MessageBox.Show("Point Index Not In Range!");
            }
        }

        private void buttonMoveP2_Click(object sender, EventArgs e)
        {
            int p;
            if (int.TryParse(textBoxP2.Text, out p)
                && p >= 10 && p < 200)
            {
                DensoControl.Only().Robot.MoveP(2, p);
            }
            else
            {
                MessageBox.Show("Point Index Not In Range!");
            }
        }

        private void buttonMoveJ1_Click(object sender, EventArgs e)
        {
            int p;
            if (int.TryParse(textBoxJ1.Text, out p)
                && p >= 10 && p < 100)
            {
                DensoControl.Only().Robot.MoveJ(1, p);
            }
            else
            {
                MessageBox.Show("Point Index Not In Range!");
            }
        }

        private void buttonMoveJ2_Click(object sender, EventArgs e)
        {
            int p;
            if (int.TryParse(textBoxJ2.Text, out p)
                && p >= 10 && p < 100)
            {
                DensoControl.Only().Robot.MoveJ(2, p);
            }
            else
            {
                MessageBox.Show("Point Index Not In Range!");
            }
        }

        #endregion

        #region robot manual

        private void buttonMoveXDec_Click(object sender, EventArgs e)
        {
            int step;
            if (int.TryParse(textBoxXStep.Text, out step))
            {
                if (isXyzMode)
                {
                    if (step > 0 && step < 500)
                    {
                        DensoControl.Only().MoveObjectPos(1, 1, -step);
                        return;
                    }
                }
                else
                {
                    if (step > 0 && step < 30)
                    {
                        DensoControl.Only().MoveObjectAng(1, 1, -step);
                        return;
                    }
                }
            }


            MessageBox.Show("Step not in Range!");
        }

        private void buttonMoveXInc_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                int step;
                if (int.TryParse(textBoxXStep.Text, out step))
                {
                    if (isXyzMode)
                    {
                        if (step > 0 && step < 500)
                        {
                            DensoControl.Only().MoveObjectPos(1, 1, step);
                            return;
                        }
                    }
                    else
                    {
                        if (step > 0 && step < 30)
                        {
                            DensoControl.Only().MoveObjectAng(1, 1, step);
                            return;
                        }
                    }
                }

                MessageBox.Show("Step not in Range!");
            });
        }

        private void buttonMoveYDec_Click(object sender, EventArgs e)
        {
            int step;
            if (int.TryParse(textBoxYStep.Text, out step))
            {
                if (isXyzMode)
                {
                    //if (step > 0 && step < 500)
                    {
                        DensoControl.Only().MoveObjectPos(1, 2, -step);
                        return;
                    }
                }
                if (step > 0 && step < 30)
                {
                    DensoControl.Only().MoveObjectAng(1, 2, -step);
                    return;
                }
            }


            MessageBox.Show("Step not in Range!");
        }

        private void buttonMoveYInc_Click(object sender, EventArgs e)
        {
            int step;
            if (int.TryParse(textBoxYStep.Text, out step))
            {
                if (isXyzMode)
                {
                    //if (step > 0 && step < 500)
                    {
                        DensoControl.Only().MoveObjectPos(1, 2, step);
                        return;
                    }
                }
                if (step > 0 && step < 30)
                {
                    DensoControl.Only().MoveObjectAng(1, 2, step);
                    return;
                }
            }


            MessageBox.Show("Step not in Range!");
        }

        private void buttonMoveZDec_Click(object sender, EventArgs e)
        {
            int step;
            if (int.TryParse(textBoxZStep.Text, out step))
            {
                if (isXyzMode)
                {
                    if (step > 0 && step < 500)
                    {
                        DensoControl.Only().MoveObjectPos(1, 3, -step);
                        return;
                    }
                }
                else
                {
                    if (step > 0 && step < 30)
                    {
                        DensoControl.Only().MoveObjectAng(1, 3, -step);
                        return;
                    }
                }
            }


            MessageBox.Show("Step not in Range!");
        }

        private void buttonMoveZInc_Click(object sender, EventArgs e)
        {
            int step;
            if (int.TryParse(textBoxZStep.Text, out step))
            {
                if (isXyzMode)
                {
                    if (step > 0 && step < 500)
                    {
                        DensoControl.Only().MoveObjectPos(1, 3, step);
                        return;
                    }
                }
                else
                {
                    if (step > 0 && step < 30)
                    {
                        DensoControl.Only().MoveObjectAng(1, 3, step);
                        return;
                    }
                }
            }


            MessageBox.Show("Step not in Range!");
        }

        private void buttonRotateInc_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (isXyzMode)
            {
                //xyz mode
                if (btn.Text.Contains("X"))
                {
                    DensoControl.Only().Rotate1(1, int.Parse(textBoxXRotateStep.Text));
                }
                if (btn.Text.Contains("Y"))
                {
                    DensoControl.Only().Rotate1(2, int.Parse(textBoxYRotateStep.Text));
                }
                if (btn.Text.Contains("Z"))
                {
                    DensoControl.Only().Rotate1(3, int.Parse(textBoxZRotateStep.Text));
                }
            }
            else
            {
                //joint mode
                if (btn.Text.Contains("4"))
                {
                    DensoControl.Only().MoveObjectAng(1, 4, int.Parse(textBoxXRotateStep.Text));
                }
                if (btn.Text.Contains("5"))
                {
                    DensoControl.Only().MoveObjectAng(1, 5, int.Parse(textBoxYRotateStep.Text));
                }
                if (btn.Text.Contains("6"))
                {
                    DensoControl.Only().MoveObjectAng(1, 6, int.Parse(textBoxZRotateStep.Text));
                }
            }
        }

        private void buttonRotateDec_Click(object sender, EventArgs e)
        {
            var btn = (Button) sender;

            if (isXyzMode)
            {
                if (btn.Text.Contains("X"))
                {
                    DensoControl.Only().Rotate1(1, -int.Parse(textBoxXRotateStep.Text));
                }
                if (btn.Text.Contains("Y"))
                {
                    DensoControl.Only().Rotate1(2, -int.Parse(textBoxYRotateStep.Text));
                }
                if (btn.Text.Contains("Z"))
                {
                    DensoControl.Only().Rotate1(3, -int.Parse(textBoxZRotateStep.Text));
                }
            }
            else
            {
                if (btn.Text.Contains("4"))
                {
                    DensoControl.Only().MoveObjectAng(1, 4, -int.Parse(textBoxXRotateStep.Text));
                }
                if (btn.Text.Contains("5"))
                {
                    DensoControl.Only().MoveObjectAng(1, 5, -int.Parse(textBoxYRotateStep.Text));
                }
                if (btn.Text.Contains("6"))
                {
                    DensoControl.Only().MoveObjectAng(1, 6, -int.Parse(textBoxZRotateStep.Text));
                }
            }
        }


        private void buttonClearError_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Move Robot Home?", "Home",
            //    MessageBoxButtons.OKCancel,
            //    MessageBoxIcon.Question,
            //    MessageBoxDefaultButton.Button1)
            //    == DialogResult.OK)
            //{
            //    DensoControl.Only().GoHome();
            //}

            DensoControl.Only().Controller.ClearError();
            DensoControl.Only().TakeArm();
        }

        private void radioButtonMotionMode_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonJointMode.Checked)
            {
                radioButtonXYZMode.Checked = false;
                isXyzMode = false;


                buttonMoveXDec.Text = "1--";
                buttonMoveYDec.Text = "2--";
                buttonMoveZDec.Text = "3--";
                buttonRotateXDec.Text = "4--";
                buttonRotateYDec.Text = "5--";
                buttonRotateZDec.Text = "6--";
                buttonMoveXInc.Text = "1++";
                buttonMoveYInc.Text = "2++";
                buttonMoveZInc.Text = "3++";
                buttonRotateXInc.Text = "4++";
                buttonRotateYInc.Text = "5++";
                buttonRotateZInc.Text = "6++";


                textBoxXStep.Text = "10";
                textBoxYStep.Text = "10";
                textBoxZStep.Text = "10";
                textBoxXRotateStep.Text = "10";
                textBoxYRotateStep.Text = "10";
                textBoxZRotateStep.Text = "10";
            }

            if (radioButtonXYZMode.Checked)
            {
                radioButtonJointMode.Checked = false;
                isXyzMode = true;


                buttonMoveXDec.Text = "X--";
                buttonMoveYDec.Text = "Y--";
                buttonMoveZDec.Text = "Z--";
                buttonRotateXDec.Text = "RX--";
                buttonRotateYDec.Text = "RY--";
                buttonRotateZDec.Text = "RZ--";
                buttonMoveXInc.Text = "X++";
                buttonMoveYInc.Text = "Y++";
                buttonMoveZInc.Text = "Z++";
                buttonRotateXInc.Text = "RX++";
                buttonRotateYInc.Text = "RY++";
                buttonRotateZInc.Text = "RZ++";


                textBoxXStep.Text = "50";
                textBoxYStep.Text = "50";
                textBoxZStep.Text = "50";
                textBoxXRotateStep.Text = "10";
                textBoxYRotateStep.Text = "10";
                textBoxZRotateStep.Text = "10";
            }
        }

        private void buttonTeach_Click(object sender, EventArgs e)
        {
            if (DensoControl.Only().Controller.ControllerPointsPVars.ContainsKey(comboBoxPointIndex.Text))
            {
                DensoControl.Only().Controller.ControllerPointsPVars[comboBoxPointIndex.Text].Value
                    = DensoControl.Only().Robot.CurPosVar.Value;

                MessageBox.Show("Teach " + comboBoxPointIndex.Text + ":" +
                                DensoControl.GetPString(DensoControl.Only().Robot.CurPosVar));
            }

            if (DensoControl.Only().Controller.ControllerPointsJVars.ContainsKey(comboBoxJointIndex.Text))
            {
                DensoControl.Only().Controller.ControllerPointsJVars[comboBoxJointIndex.Text].Value
                    = DensoControl.Only().Robot.CurAngVar.Value;

                MessageBox.Show("Teach " + comboBoxJointIndex.Text + ":" +
                                DensoControl.GetJString(DensoControl.Only().Robot.CurAngVar));
            }
        }

        #endregion
    }
}