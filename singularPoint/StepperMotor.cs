using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace parachute
{
    using UIM512Driver;

    class StepperMotor
    {
        public int nodeAddr = -1;
        public int row = -1;
        public int col = -1;
        public int gateAddr = -1;
        public string gateName;
        public bool foundInDriver;

        bool enabled;
        int currentSpeedValue;
        int currentAccValue;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public StepperMotor()
        {
            enabled = false;
            currentSpeedValue = -1;
            currentAccValue = -1;
            signalStopEnabled = false;
        }

        public void enable()
        {
            if (gateAddr == -1 || nodeAddr == -1 || foundInDriver == false)
            {
                return;
            }
            BASIC_ACK_OBJ objBasicAck = new BASIC_ACK_OBJ();
            if (-1 == UIM512Driver.UimENA(gateAddr, nodeAddr, true, ref objBasicAck))
            {
                enabled = false;
                log.Error("enable motor failed: gate- " + gateName + " " + gateAddr + "nodeAddr: " + nodeAddr);
            }
            else
            {
                enabled = true;
                //log.Debug("enable motor failed: gate- " + gateName + " " + gateAddr + "nodeAddr: " + nodeAddr);
            }
        }

        public void disable()
        {
            //if (enabled == false)
            //    return;
            if (foundInDriver == false)
                return;

            BASIC_ACK_OBJ objBasicAck = new BASIC_ACK_OBJ();
            if (-1 == UIM512Driver.UimOFF(gateAddr, nodeAddr, true, ref objBasicAck))
            {
                enabled = true;
                log.Error("disable motor failed: gate- " + gateName + " " + gateAddr + "nodeAddr: " + nodeAddr);
            }
            else
            {
                enabled = false;
            }
        }

        bool isEnabled()
        {
            return enabled;
        }

        public bool setSpeed(int speedValue)
        {
            //if (enabled == false)
            //{
            //    return false;
            //}
            if (foundInDriver == false)
                return false;
            int nRtnValue = -1;
            if (-1 == UIM512Driver.SetSPD(gateAddr, nodeAddr, speedValue, false, ref nRtnValue))
            {
                return false;
            }
            currentSpeedValue = speedValue;
            return true;
        }

        public void setAcc(int acc)
        {
            //if (enabled == false)
            //{
            //    return;
            //}
            if (foundInDriver == false)
                return;

            int nRtnValue = -1;
            if(-1 == UIM512Driver.SetmACC(gateAddr, nodeAddr, acc, true, ref nRtnValue))
            {
                return;
            }
            currentAccValue = acc;
        }

        const int WAIT_TIME_OUT_SECOND = 20;
        public void waitToDesiredSpeed(int desiredSpeed, int waitIntervals)
        {
            //if (enabled == false)
            //    return;
            if (!this.foundInDriver)
                return;

            uint counter = 0;
            while (true)
            {
                if (isInSpeed(desiredSpeed))
                {
                    break;
                }
                else {
                    setSpeed(desiredSpeed);
                }
                if ((uint)(((++counter) * waitIntervals) / 1000) > WAIT_TIME_OUT_SECOND)
                {
                    //write timeout log
                    log.Warn("waitToDesiredSpeed timeout gate: " + this.gateName + " nodeAddr:" + this.nodeAddr);
                    break;
                }
                Thread.Sleep(waitIntervals);
            }
        }

        public bool isInSpeed(int speedValue)
        {
            //if (enabled == false)
            //    return false;
            if (foundInDriver == false)
                return false;
            //if (currentSpeedValue == -1)
            //{
                getCurrentSpeed();
            //}

            if (currentSpeedValue == speedValue)
            {
                return true;
            }
            return false;
        }

        public int getCurrentSpeed()
        {
            //if (enabled == false)
            //{
            //    return (-1);
            //}
            if (foundInDriver == false)
                return (-1);

            int returnedSpeed = -1;
            if (1 == UIM512Driver.GetSPD(gateAddr, nodeAddr, ref returnedSpeed))
            {
                currentSpeedValue = returnedSpeed;
            }
            return currentSpeedValue;
        }

        public int getCurrentAcc()
        {
            //if (enabled == false)
            //{
            //    return (-1);
            //}
            if (foundInDriver == false)
                return (-1);

            int returnAcc = -1;
            if (1 == UIM512Driver.GetmACC(gateAddr, nodeAddr, ref returnAcc))
            {
                currentAccValue = returnAcc;
            }
            return currentAccValue;
        }

        bool signalStopEnabled;
        public void enableSignalStop()
        {
            if (foundInDriver == false)
                return;
            //if (enabled == false)
            //{
            //    return;
            //}
            //if (signalStopEnabled == true)
            //    return;
            P_S12CON pS12CON_OUT = new P_S12CON();
            P_S12CON pS12CON_IN = new P_S12CON();
            pS12CON_IN.uiS1FACT = 0x0;
            pS12CON_IN.uiS1RACT = 0x0;
            pS12CON_IN.uiS1FACT = 0x0;
            pS12CON_IN.uiS1RACT = 0x3;
            if (-1 == UIM512Driver.SetS12CON(gateAddr, nodeAddr, ref pS12CON_IN, false, ref pS12CON_OUT))
            {
                Console.WriteLine("enableSignalStop failed gate:{0}, node:{1}",gateAddr,nodeAddr); ;
            }
            signalStopEnabled = true;
        }

        public void disableSignalStop()
        {
            //if (enabled == false)
            //{
            //    return;
            //}
            if (foundInDriver == false)
                return;
            //if (signalStopEnabled == false)
            //    return;
            P_S12CON pS12CON_OUT = new P_S12CON();
            P_S12CON pS12CON_IN = new P_S12CON();
            pS12CON_IN.uiS1FACT = 0x0;
            pS12CON_IN.uiS1RACT = 0x0;
            pS12CON_IN.uiS1FACT = 0x0;
            pS12CON_IN.uiS1RACT = 0x0;
            UIM512Driver.SetS12CON(gateAddr, nodeAddr, ref pS12CON_IN, false, ref pS12CON_OUT);
            signalStopEnabled = false;
        }

        public void speedUp(int deltaVal)
        {
            if (enabled == false)
            {
                return;
            }
            int tmpSpeed = currentSpeedValue + deltaVal;
            if (true == setSpeed(tmpSpeed))
            {
                this.currentSpeedValue = tmpSpeed;
            }
        }
        public void speedDown(int deltaVal)
        {
            if (enabled == false)
            {
                return;
            }
            int tmpSpeed = currentSpeedValue - deltaVal;
            if ( true == setSpeed(tmpSpeed))
            {
                this.currentSpeedValue = tmpSpeed;
            }
        }
    }

    
}
