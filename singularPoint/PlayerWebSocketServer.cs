using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;

namespace parachute
{
    class PlayerWebSocketServer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void OnClose(UserContext aContext)
        {
            Console.WriteLine("Websocket closed.");
        }

        public static void OnError(UserContext aContext)
        {
            Console.WriteLine("Websocket error.");
        }

        public static void OnMessage(UserContext aContext)
        {
            // use e.Data, parse data and determin what to do
            String strdata = aContext.DataFrame.ToString();
            Console.WriteLine("Websocket recv: " + strdata);
            try
            {
                PlayTask pTask = JsonConvert.DeserializeObject<PlayTask>(strdata);
                //if (pTask.actionType.Equals("stopPlayDefaultList"))
                {
                //    MatrixPlayer.stopPlayDefaultList();
                //    Console.WriteLine("recv stopPlayDefaultList");    
                //    return;
                }
                MatrixPlayer.printPlayTask(pTask);
                
                ///////////////////////////////////////////for immediate shutdown.
                if (pTask.actionType.Equals("shutdown"))
                {
                    log.Debug("!!! shutdown cmd received. do it immediately.");
                    //do the shutdown job directly.
                    if (MatrixPlayer.kShutdownDetected != null)
                    {
                        MatrixPlayer.stopLoop = true;
                        //stop(m);
                        MatrixPlayer.kShutdownDetected();
                    }
                }
                else {
                    MatrixPlayer.addPlayTask(pTask);
                }
                
            }catch(Exception ex){
                Console.WriteLine("错误解析: "+ex.Message);
            }
        }

        public static void onConnected(UserContext aContext)
        {
            Console.WriteLine("Websocket open Succeed.");
        }
        
    }
}
