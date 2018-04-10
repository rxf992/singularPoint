using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alchemy;
using Alchemy.Classes;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices; 

namespace stepperMatrix
{
    using parachute;
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 
        static public MotorMatrix matrix = null;
        public static MainForm mf;
        static WebSocketServer server = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void MyHandler(object sender, UnhandledExceptionEventArgs e)
        {
            //log.Fatal(e);
            //...
            log.Fatal("!!! rxf unhandled exception");
            log.Fatal(e.ToString());
        }
        [STAThread]
        static void Main()
        {
            log.Info("Hello logging world!");
            needPowerOff = false;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            //Thread.Sleep(5000);
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string fname = path + "\\MatrixConfig.matrix";
            //Alchemy.WebSocketServer;
            
                      
            matrix = MotorMatrix.createMotorMatrixWithConfigFile(fname);            
            MatrixPlayer.kShutdownDetected = ShutdownMachine;

            
            if (matrix != null)
            {
                MatrixPlayer.start(matrix);
                MatrixPlayer.setDefaultMatrix(matrix);
                
                
            }

            //string localIP = GetInternalIP();
            //string url = "ws://"+localIP;
            //Console.WriteLine("ws url: "+url);
            // start websocket server with ssl
            server = new WebSocketServer(25532, IPAddress.Any)
            {
                OnReceive = PlayerWebSocketServer.OnMessage,
                //OnSend = onSend,
                //OnConnect = PlayerWebSocketServer.onConnected,
                OnConnected = PlayerWebSocketServer.onConnected,
                OnDisconnect = PlayerWebSocketServer.OnClose,
                
                TimeOut = new TimeSpan(0, 5, 0)
            };
            
            //server.AddWebSocketService<PlayerWebSocketServer>("/PlayerWebSocketServer");
            server.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mf = new MainForm();
            Application.Run(mf);
            Console.WriteLine("exit application.");
            log.Debug("!!! mf closed !!!");
            server.Stop();
            MatrixPlayer.stop(matrix);
            log.Debug("!!! needPowerOff=" + needPowerOff);
            if(needPowerOff)
            {
                log.Debug("do shut down");
                //Process myProcess = new Process();
                //myProcess.StartInfo.FileName = "cmd.exe";
                //myProcess.StartInfo.UseShellExecute = false;
                //myProcess.StartInfo.RedirectStandardInput = true;
                //myProcess.StartInfo.RedirectStandardOutput = true;
                //myProcess.StartInfo.RedirectStandardError = true;
                //myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.Start();
                ////myProcess.StandardInput.WriteLine("shutdown -s -f -t 5");
                //myProcess.StandardInput.WriteLine("shutdown -p -f");
                //myProcess.Close();
                Process.Start("shutdown", "/p");    // starts the shutdown application 
                // the argument /s is to shut down the computer
                // the argument /t 0 is to tell the process that 
                // the specified operation needs to be completed 
                // after 0 seconds
                Thread.Sleep(3000);
                
            }

        }

        static void ShutdownMachine()
        {
            Console.WriteLine("shutdown");
            log.Debug("shutdown machine !!!");
            needPowerOff = true;
            //server.Stop();
            //if (matrix != null)
            //{
            //    matrix.setSpeedMotorsAll(0);
            //    matrix.disableMotorsAll();
            //}
          
            //if(mf != null) mf.Close();
            log.Debug("closeed main foram!!!");
            System.Windows.Forms.Application.Exit();
        }

        static public string GetInternalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string localIP = "?";
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString().Equals("InterNetwork"))
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public static bool needPowerOff { get; set; }
    }
}
