using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Collections;

namespace stepperMatrix
{
    using parachute;
    public partial class MainForm : Form
    {             

        public MainForm()
        {
            InitializeComponent();
            //initNodeAddrMap();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }
                

        
        private void textBoxRow_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool b1 = !Char.IsNumber(e.KeyChar);
            bool b2 = e.KeyChar != (char)8;
            if(false == b1 && b2)
            {
                //e.Handled = true;
            }
            else if (b2 == false)
            {

            }
            else
            {
                outputLog("请输入数字!!!");
                e.Handled = true;
            }
        }

        public void outputLog(string log)
        {
            
            if (textBox_RTCN.GetLineFromCharIndex(textBox_RTCN.Text.Length) > 1000)
                textBox_RTCN.Text = "";
            textBox_RTCN.AppendText(DateTime.Now.ToString("HH:mm:ss ") + log + "\r\n");
        }

        private void textBoxCol_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool b1 = !Char.IsNumber(e.KeyChar);
            bool b2 = e.KeyChar != (char)8;
            if (false == b1 && b2)
            {
                //e.Handled = true;
            }
            else if (b2 == false)
            {

            }
            else
            {
                outputLog("请输入数字!!!");
                e.Handled = true;
            }
        }

        private void textBoxSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool b1 = !Char.IsNumber(e.KeyChar);
            bool b2 = e.KeyChar != (char)8;
            if (false == b1 && b2)
            {
                //e.Handled = true;
            }
            else if (b2 == false)
            {

            }
            else if (e.KeyChar == '-')
            {

            }
            else if (e.KeyChar == '+')
            {

            }
            else
            {
                outputLog("请输入数字!!!");
                e.Handled = true;
            }
        }

        private void textBoxACC_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool b1 = !Char.IsNumber(e.KeyChar);
            bool b2 = e.KeyChar != (char)8;
            if (false == b1 && b2)
            {
                //e.Handled = true;
            }
            else if (b2 == false)
            {

            }
            else if (e.KeyChar == '-')
            {

            }
            else if (e.KeyChar == '+')
            {

            }
            else
            {
                outputLog("请输入数字!!!");
                e.Handled = true;
            }
        }

        private void checkMatrix_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked == true)
            {
                checkRow.Enabled = false;
                textBoxRow.Enabled = false;
                textBoxCol.Enabled = false;
            }
            else
            {
                checkRow.Enabled = true;
                textBoxRow.Enabled = true;
                if (checkRow.Checked == true)
                {
                    textBoxCol.Enabled = false;
                }
                else
                {
                    textBoxCol.Enabled = true;
                }                
            }
        }

        private void checkRow_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked == true)
            {
                textBoxCol.Enabled = false;
            }
            else
            {
                textBoxCol.Enabled = true;
            }
        }

        private void showMap_Click(object sender, EventArgs e)
        {
            //printNodeAddrMap();
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            //string fname = path + "\\MatrixConfig.matrix";
            string fstr = DateTime.Now.ToLongTimeString();
            fstr = fstr.Replace(":", "-");
            string fname = path+String.Format("\\matrixMap-{0}.log", fstr);         
            string fname2 = path+String.Format("\\matrixUnfindMotors-{0}.log", fstr);
            Program.matrix.printMatrix(fname);
            Program.matrix.printUnFindMotor(fname2);
            outputLog("矩阵信息已保存为："+fname);
            outputLog("矩阵中未找到的点信息已保存为为：" + fname);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (checkMatrix.Checked == true)
            {                
                Program.matrix.disableMotorsAll();
                outputLog("全矩阵电机关闭.");

                //同时关闭后台矩阵播放
                PlayTask task = new PlayTask();
                task.actionType = "stopPlayDefaultList";
                task.col = -1;
                task.row = -1;
                task.value = -1;
                task.lastTime = 10000;
                MatrixPlayer.addPlayTask(task);
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == true)
            {
                int row = Int32.Parse(textBoxRow.Text);                
                Program.matrix.disableMotorsRow(row);
                outputLog("关闭第 " + row + " 行电机.");
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == false)
            {
                int row = Int32.Parse(textBoxRow.Text);
                int col = Int32.Parse(textBoxCol.Text);
                if (row == 0)
                {
                    outputLog("行从1开始，不能为 0");
                    return;
                }
                if (col == 0)
                {
                    outputLog("列从1开始，不能为 0");
                    return;
                }                
                Program.matrix.disableMotor(row,col);
                outputLog("关闭 第 " + row + " 行, 第 " + col + " 列 电机.");
            }
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            if (checkMatrix.Checked == true)
            {
                Program.matrix.enableMotorsAll();
                outputLog("全矩阵电机打开.");
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == true)
            {
                int row = Int32.Parse(textBoxRow.Text);
                Program.matrix.enableMotorsRow(row);
                outputLog("打开第 " + row + " 行电机.");
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == false)
            {
                int row = Int32.Parse(textBoxRow.Text);
                int col = Int32.Parse(textBoxCol.Text);
                if (row == 0)
                {
                    outputLog("行从1开始，不能为 0");
                    return;
                }
                if (col == 0)
                {
                    outputLog("列从1开始，不能为 0");
                    return;
                }
                Program.matrix.enableMotor(row, col);
                outputLog("打开 第 " + row + " 行, 第 " + col + " 列 电机.");
            }
        }

        private void btnSetSpeed_Click(object sender, EventArgs e)
        {
            int speed = Int32.Parse(textBoxSpeed.Text);
            if(checkMatrix.Checked == true){
                Program.matrix.setSpeedMotorsAll(speed);
                outputLog("设置全矩阵速度为：" + speed);
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == true)
            {
                int row = Int32.Parse(textBoxRow.Text);
                Program.matrix.setSpeedMotorsRow(row, speed);
                outputLog("设置第 "+ row +" 行速度为：" + speed);
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == false)
            {
                int row = Int32.Parse(textBoxRow.Text);
                int col = Int32.Parse(textBoxCol.Text);
                if (row == 0)
                {
                    outputLog("行从1开始，不能为 0");
                    return;
                }
                if (col == 0)
                {
                    outputLog("列从1开始，不能为 0");
                    return;
                }
                Program.matrix.setSpeedMotor(row, col, speed);                
                outputLog("设置 第 " + row + " 行, 第 "+col+" 列 速度为：" + speed);
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {            
            if (checkMatrix.Checked == true)
            {
                MatrixPlayer.syncMatrixAll(Program.matrix);
                outputLog("设置全矩阵同步.");
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == true)
            {
                int row = Int32.Parse(textBoxRow.Text);                
                MatrixPlayer.syncMatrixRow(Program.matrix, row);
                outputLog("设置第 " + row + " 行同步.");
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == false)
            {
                int row = Int32.Parse(textBoxRow.Text);
                int col = Int32.Parse(textBoxCol.Text);
                if (row == 0)
                {
                    outputLog("行从1开始，不能为 0");
                    return;
                }
                if (col == 0)
                {
                    outputLog("列从1开始，不能为 0");
                    return;
                }                
                MatrixPlayer.syncMatrixStepMotor(Program.matrix, row, col);
                outputLog("设置 第 " + row + " 行, 第 " + col + " 列 同步.");
            }            
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"\\";
            openFileDialog.Filter = "*.*|播放文件|*.lst|";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<PlayTask> plist = new List<PlayTask>();
                MatrixPlayer.initPlayList(openFileDialog.FileName, plist);
                if (plist.Count > 0)
                {
                    MatrixPlayer.stopPlayDefaultList();
                }
                foreach (PlayTask task in plist)
                {
                    MatrixPlayer.addPlayTask(task);
                }
            }
        }

        private void btnLoadMatrix_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"\\";
            openFileDialog.Filter = "*.*|矩阵文件|*.matrix|";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                Program.matrix = MotorMatrix.createMotorMatrixWithConfigFile(fName);
                if (Program.matrix != null)
                {
                    outputLog("从文件 " + fName + " 创建矩阵成功.");
                }
                else
                {
                    outputLog("从文件 " + fName + " 创建矩阵失败，请检查矩阵定义文件.");
                }
            }
        }

        private void btnSetAcc_Click(object sender, EventArgs e)
        {
            int acc = Int32.Parse(this.textBoxACC.Text);
            if (checkMatrix.Checked == true)
            {
                Program.matrix.setAccMotorsAll(acc);
                outputLog("设置全矩阵加速度为：" + acc);
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == true)
            {
                int row = Int32.Parse(textBoxRow.Text);
                Program.matrix.setAccMotorsRow(row, acc);
                outputLog("设置第 " + row + " 行加速度为：" + acc);
            }
            else if (checkMatrix.Checked == false && checkRow.Checked == false)
            {
                int row = Int32.Parse(textBoxRow.Text);
                int col = Int32.Parse(textBoxCol.Text);
                if (row == 0)
                {
                    outputLog("行从1开始，不能为 0");
                    return;
                }
                if (col == 0)
                {
                    outputLog("列从1开始，不能为 0");
                    return;
                }
                Program.matrix.setAccMotor(row, col, acc);
                outputLog("设置 第 " + row + " 行, 第 " + col + " 列 加速度为：" + acc);
            }
        }

        private void btnSpeedUp_Click(object sender, EventArgs e)
        {

        }

        private void btnSpeedDown_Click(object sender, EventArgs e)
        {

        }





      


    }
}
