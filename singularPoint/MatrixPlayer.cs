using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;


namespace parachute
{
    using stepperMatrix;
    struct PlayTask
    {
        public int row;
        public int col;
        public int lastTime;
        public string actionType;
        public int value;
    }     

    class MatrixPlayer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static MotorMatrix m=null;
        static public void setDefaultMatrix(MotorMatrix mm){
            if (mm == null) return;
            m = mm;
            m.enableMotorsAll();
        }

        static public void printPlayTask(PlayTask task)
        {
            Console.WriteLine("row:　" + task.row);
            Console.WriteLine("col:　" + task.col);
            Console.WriteLine("lastTime:　" + task.lastTime);
            Console.WriteLine("actionType:　" + task.actionType);
            Console.WriteLine("value:　" + task.value);

            log.Debug("New PlayTask" + "actionType:　" + task.actionType + "row:　" + task.row + "value:　" + task.value + "lastTime:　" + task.lastTime);
        }

        public static void start(MotorMatrix matrix)
        {
            if (matrix != null)
            {
                matrix.enableMotorsAll();
            }
            
            Thread thread = new Thread(new ParameterizedThreadStart(MatrixPlayer.playRound));
            thread.Start(null);
            Thread.Sleep(3000);
            PlayTask task = new PlayTask();
            task.actionType = "startPlayDefaultList";
            task.col = -1;
            task.row = -1;
            task.value = -1;
            task.lastTime = 10000;
            addPlayTask(task);
        }

        public static void stop(MotorMatrix matrix)
        {
            if (matrix != null)
            {
                matrix.disableMotorsAll();
            }
            stopLoop = true;

            try
            {
                ////////////////FIXME ///////////////
                // this will always cause exception. some where alway relaese the sem.
                sem.Release();
                
            }
            catch (Exception e)
            {
                log.Warn("stop() sem.Release Exceptrion: " + e.Message + e.StackTrace);
            }
        }

        static public void addPlayTask(PlayTask task)
        {
            //Program.mf.outputLog("add task: "+task.actionType);
            mtx.WaitOne();
            playTasks.Enqueue(task);
            mtx.ReleaseMutex();
            /////////////////////FIXME//////////////
            //if (isPlaying == false) {
            //    try
            //    {
            //        sem.Release();
            //    }
            //    catch(Exception e)
            //    {
            //        log.Warn("addPlayTask() sem.Release Exceptrion: " + e.Message + e.StackTrace);
            //    }
                
            //}
                
            ///////////////////////////////////////
        }

        static bool isPlaying = false;
        static bool stopLoop = false;
        static Semaphore sem = new Semaphore(0,1);
        static Queue<PlayTask> playTasks = new Queue<PlayTask>();
        static Mutex mtx = new Mutex();
        static bool playDefaultList = false;
        static int defaultPlayListIndex = 0;
        static List<PlayTask> defaultPlayList = new List<PlayTask>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private static void playRound(Object obj)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string fstr = DateTime.Now.ToLongTimeString();
            fstr = fstr.Replace(":", "-");
            string fname = path + String.Format("\\playLog-{0}.log", fstr);
            StreamWriter log = new StreamWriter(fname);
            Thread.Sleep(5000);//wait some time to let user can kill the program before it actaully runs.
            while (false == stopLoop)
            {
                if (playTasks.Count == 0)
                    sem.WaitOne();
                if (playTasks.Count == 0)
                    continue;
                isPlaying = true;
                mtx.WaitOne();
                PlayTask task = playTasks.Dequeue();
                //Console.WriteLine("pop one task, {0} tasks left",playTasks.Count);
                
                string infostr = String.Format("play: {0} with row:{1}, col:{2}, value:{3}, {4} tasks left.",
                    task.actionType, task.row, task.col, task.value, playTasks.Count);
                log.WriteLine(infostr);
                Console.WriteLine(infostr);
                //Program.mf.outputLog(infostr);
                log.Flush();
                mtx.ReleaseMutex();
                playTask(task);
                if (playDefaultList && defaultPlayList.Count > 0)
                {
                    defaultPlayListIndex ++;
                    if (defaultPlayListIndex == defaultPlayList.Count)
                    {
                        defaultPlayListIndex = 0;
                    }
                    PlayTask t = defaultPlayList[defaultPlayListIndex];
                    addPlayTask(t);
                }
                isPlaying = false;
            }
            Console.WriteLine("player stoped.");
            //log.Inf
        }

        public delegate void ShutDownDetected();
        public static ShutDownDetected kShutdownDetected = null;

        static void playTask(PlayTask task)
        {
            if (task.actionType.Equals("setSpeed"))
            {        
                if (task.row < 0 && task.col < 0)
                {
                    if (m != null)
                    {
                        m.setSpeedMotorsAll(task.value);                        
                    }
                }
                else if (task.row > 0 && task.col < 0)
                {
                    if (m != null)
                    {
                        m.setSpeedMotorsRow(task.row, task.value);                        
                    }
                }
                else if (task.row > 0 && task.col > 0)
                {
                    if (m != null)
                    {
                        m.setSpeedMotor(task.row,
                            task.col,
                            task.value);                        
                    }
                }
            }
                    
            else if( task.actionType.Equals("brake"))
            {
                if (task.row < 0 && task.col < 0)
                {
                    if (m != null)
                    {
                        m.setSpeedMotorsAll(0);                        
                    }
                }
                else if (task.row > 0 && task.col < 0)
                {
                    if (m != null)
                    {
                        m.setSpeedMotorsRow(task.row, 0);
                    }
                }
                else if (task.row > 0 && task.col > 0)
                {
                    if (m != null)
                    {
                        m.setSpeedMotor(task.row,
                            task.col,
                            0);
                    }
                }
            }
            else if(task.actionType.Equals("shutdown"))
            {
                if (kShutdownDetected != null)
                {
                    stopLoop = true;
                    //stop(m);
                    kShutdownDetected();
                }                     
            }
            else if(task.actionType.Equals("startPlayDefaultList"))
            {
                playDefaultList = true;
                initDefaultPlayList();
                if (defaultPlayList.Count > 0)
                {
                    defaultPlayListIndex = 0;
                    addPlayTask(defaultPlayList[defaultPlayListIndex]);
                }
            }
            else if (task.actionType.Equals("stopPlayDefaultList"))
            {
                stopPlayDefaultList();
            }
            else if(task.actionType.Equals("wait"))
            {
                if(task.value > 0)
                    Thread.Sleep(task.value);
            }
            else if (task.actionType.Equals("waitSpeed"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {
                    m.waitDesiredSpeedAll(task.value);
                }
                else if (task.row > 0 && task.col < 0)
                {
                    m.waitDesiredSpeedRow(task.row, task.value);
                }
                else if (task.row > 0 && task.col > 0)
                {
                    m.waitDesiredSpeed(task.row, task.col, task.value);
                }
            }
            else if (task.actionType.Equals("disable"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {                   
                    m.disableMotorsAll();                    
                }
                else if (task.row > 0 && task.col < 0)
                {
                    m.disableMotorsRow(task.row);                    
                }
                else if (task.row > 0 && task.col > 0)
                {                    
                    m.disableMotor(task.row, task.col);                            
                }
            }
            else if (task.actionType.Equals("enable"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {
                    m.enableMotorsAll();
                }
                else if (task.row > 0 && task.col < 0)
                {
                    m.enableMotorsRow(task.row);
                }
                else if (task.row > 0 && task.col > 0)
                {
                    m.enableMotor(task.row, task.col);
                }
            }
            else if (task.actionType.Equals("sync"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {
                    m.enableSignalStopAll();
                    m.waitDesiredSpeedAll(0);
                }
                else if (task.row > 0 && task.col < 0)
                {
                    m.enableSignalStopRow(task.row);
                    m.waitDesiredSpeedRow(task.row,0);
                }
                else if (task.row > 0 && task.col > 0)
                {
                    m.enableSignaleStop(task.row, task.col);
                    m.waitDesiredSpeed(task.row, task.col, 0);
                }
            }
            else if (task.actionType.Equals("random"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {
                    playRandomAll(false);
                }
                else if (task.row > 0 && task.col < 0)
                {
                    playRandomRow(task.row, false);
                }
                else if (task.row > 0 && task.col > 0)
                {
                    playRandom(task.row, task.col, false);
                }
            }
            else if (task.actionType.Equals("random-slowly"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {
                    playRandomAll(true);
                }
                else if (task.row > 0 && task.col < 0)
                {
                    playRandomRow(task.row, true);
                }
                else if (task.row > 0 && task.col > 0)
                {
                    playRandom(task.row, task.col, true);
                }
            }
            else if (task.actionType.Equals("enableSync"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {
                    m.enableSignalStopAll();
                }
                else if (task.row > 0 && task.col < 0)
                {
                    m.enableSignalStopRow(task.row);
                }
                else if (task.row > 0 && task.col > 0)
                {
                    m.enableSignaleStop(task.row, task.col);
                }
            }
            else if (task.actionType.Equals("disableSync"))
            {
                if (m == null) return;
                if (task.row < 0 && task.col < 0)
                {
                    m.disableSignalStopAll();
                }
                else if (task.row > 0 && task.col < 0)
                {
                    m.disableSignalStopRow(task.row);
                }
                else if (task.row > 0 && task.col > 0)
                {
                    m.disableSignaleStop(task.row, task.col);
                }
            }
        }

        static public void playRandomAll(bool slow)
        {
            if (m == null) return;
            m.enableMotorsAll();
            if (slow)
            {
                m.employOperationOnMotorsAll(setRandomSpeedSlow);
            }
            else
            {
                m.employOperationOnMotorsAll(setRandomSpeed);
            }
        }

        static public void playRandomSelectFromAll(int selectMotorNum, bool slowSpeed)
        { 
            //随机挑选N个电机，以某个随机的速度运行 
            if (m == null) return;
            

        }
        static public void playRandomRow(int row, bool slow)
        {
            if (m == null) return;
            m.enableMotorsRow(row);
            if (slow)
            {
                m.employOperationOnMotorsRow(row, setRandomSpeedSlow);
            }
            else {
                m.employOperationOnMotorsRow(row, setRandomSpeed);
            }
            
        }

        static public void playRandom(int row, int col, bool slow)
        {
            if (m == null) return;
            m.enableMotor(row, col);
            if (slow) {
                m.employOperationOnMotor(row, col, setRandomSpeedSlow); 
            } else {
                m.employOperationOnMotor(row, col, setRandomSpeed); 
            }

        }

        static int[] speedSequence = { 
                                         1000, 2000, 2000, 3000, 
                                         2000, 6000, 2000, 3000,
                                         -1000,-2000,-2000, -3000, 
                                         -1000,-6000, -3000, -3000
                                     };
        static int[] speedSlowSequence = { 
                                         1000, 1500, 2000, 2500, 
                                         3000, 1500, 3000, 1500,
                                         -1000,-1500,-2000, -2500, 
                                         -1000,-1500, -3000, -1500
                                     };
        static Random rd = new Random(7);

        static public void setRandomSpeed(StepperMotor motor)
        {            
            int index = rd.Next(0, 15);
            motor.setSpeed(speedSequence[index]);
        }
        static public void setRandomSpeedSlow(StepperMotor motor)
        {
            int index = rd.Next(0, 15);
            motor.setSpeed(speedSlowSequence[index]);
        }
        static void initDefaultPlayList()
        {
            // todo
            defaultPlayList.Clear();
            string str = System.Environment.CurrentDirectory + "/DefaultPlayList.lst";
            initPlayList(str, defaultPlayList);
            
        }

        
        static public void stopPlayDefaultList()
        {
            playDefaultList = false;
            if (m != null)
            {
                playTasks.Clear();
                defaultPlayListIndex = 0;
                m.setSpeedMotorsAll(0);
            }
        }

        static public void startPlayDefaultList()
        {
            playDefaultList = true;
        }

        static public void initPlayList(string fname, List<PlayTask> plst)
        {            
            Console.WriteLine("config file: " + fname);
            if (true == File.Exists(fname))
            {
                StreamReader file = new StreamReader(fname);
                String contentstr = file.ReadLine();
                while (contentstr!=null)
                {
                    //string jsonstr = file.ReadLine();
                    Console.WriteLine("line content: " + contentstr);
                    try
                    {
                        PlayTask pTask = JsonConvert.DeserializeObject<PlayTask>(contentstr);
                        plst.Add(pTask);

                    }
                    catch (Exception e)
                    {
                        ;
                    }
                    contentstr = file.ReadLine();
                }
            }
        }

        public static void randomAllStepMptors(MotorMatrix matrix)
        {
            if (matrix == null) return;
            matrix.enableSignalStopAll();

            matrix.waitDesiredSpeedAll(2000);

            matrix.disableSignalStopAll();
        }

        public static void setSpeedAll(MotorMatrix matrix, int speedValue)
        {
            if (matrix == null) return;
            matrix.setSpeedMotorsAll(speedValue);
            matrix.waitDesiredSpeedAll(speedValue, 500);
        }

        public static void playScript(MotorMatrix matrix, string fileName)
        {
            if (matrix == null) return;
        }

        public static void syncMatrixAll(MotorMatrix matrix)
        {
            if (matrix == null) return;
            //matrix.enableMotorsAll();
            //Console.WriteLine("after enableMotorsAll.");
            matrix.setSpeedMotorsAll(1000);
            //Console.WriteLine("after setSpeedMotorsAll.");
            //matrix.waitDesiredSpeedAll(1000, 500);
            //Console.WriteLine("after waitDesiredSpeedAll.");
            //matrix.employOperationOnMotorsAll(printSpeed);
            //Console.WriteLine("after employOperationOnMotorsAll.");
            matrix.enableSignalStopAll();
            //Console.WriteLine("after enableSignalStopAll.");
            matrix.waitDesiredSpeedAll(0, 500);
            //Console.WriteLine("after waitDesiredSpeedAll.");
            matrix.disableSignalStopAll();
            //Console.WriteLine("after disableSignalStopAll.");
        }

        public static void syncMatrixRow(MotorMatrix matrix, int row)
        {
            if (matrix == null) return;
            //matrix.enableMotorsRow(row);
            matrix.setSpeedMotorsRow(row, 1000);
            //matrix.waitDesiredSpeedRow(row, 1000, 500);
            //matrix.employOperationOnMotorsRow(row, printSpeed);
            matrix.enableSignalStopRow(row);
            matrix.waitDesiredSpeedRow(row, 0, 500);
            matrix.disableSignalStopRow(row);
        }

        public static void syncMatrixStepMotor(MotorMatrix matrix, int row, int col)
        {
            if (matrix == null) return;
            StepperMotor motor = matrix.getStepperMotro(row, col);
            if (motor == null)
                return;
            motor.enableSignalStop();
            motor.waitToDesiredSpeed(0, 500);
            motor.disableSignalStop();
        }

      

        static public void printSpeed(StepperMotor motor)
        {
            Console.WriteLine("row: "+motor.row+", col: "+motor.col+", speed: "+motor.getCurrentSpeed());
        }
    }
}
