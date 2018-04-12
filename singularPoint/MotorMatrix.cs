using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using canGate;

namespace parachute
{
    class MotorMatrix
    {
        /// </summary>
        /// 真正存放网关对象的表
        /// </summary>
        Hashtable canGateMap = new Hashtable();

        /// <summary>
        /// 读取配置文件时填写的信息的表
        /// </summary>
        Hashtable canGateInfoMap = new Hashtable();
        
        /// <summary>
        /// 电机矩阵名称
        /// </summary>
        string matrixName;

        /// <summary>
        /// 电机矩阵表，存放 SetpperMotor 对象 <row, rowTalbe>
        /// rowTalbe <col, stepperMotor>
        /// </summary>
        Hashtable matrix = new Hashtable();               
        
        // 初始化 matrix
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static MotorMatrix createMotorMatrixWithConfigFile(String configFullFileName)
        {
            log.Info("createMotorMatrixWithConfigFile start");
            MotorMatrix instance = new MotorMatrix();
            if (instance == null || false == instance.init(configFullFileName))
            {
                return null;
            }
            log.Info("createMotorMatrixWithConfigFile done.");
            return instance;
        }

        bool init(String fileName)
        {
            log.Info("MotorMatrix--> init config start.");
            {
                // read config file      
                if (!File.Exists(fileName))
                {
                    return false;
                }

                StreamReader file = new StreamReader(fileName);
                JsonReader reader = new JsonTextReader(new StringReader(file.ReadToEnd()));

                while (reader.Read())
                {
                    Console.WriteLine("TokenType: " + reader.TokenType + "\t\t" + ", ValueType:" + reader.ValueType + "\t\t" + ", Value: " + reader.Value);
                    if (reader.TokenType == JsonToken.PropertyName && "projectName".Equals((string)reader.Value))
                    {
                        reader.Read();
                        matrixName = (string)reader.Value;
                    }
                    else if (reader.TokenType == JsonToken.PropertyName && "canGates".Equals((string)reader.Value))
                    {
                        reader.Read();  // gates array
                        if (reader.TokenType != JsonToken.StartArray)
                        {
                            Console.WriteLine("Wrong config file format!");
                            return false;
                        }
                        if (false == readCanGateArray(ref reader))
                            return false;
                    }
                }
            }
            log.Info("MotorMatrix--> read init config file done.");
            {
                // search from drivers and match to canGateMap
                matchDriverGatesToMatrixGateInfos();
                log.Info("MotorMatrix--> init --> matchDriverGatesToMatrixGateInfos done.");
                return true;
            }
        }

        void matchDriverGatesToMatrixGateInfos()
        {
            List<CanGate> gList = CanGate.getGateList(CanGate.UIDEV_RS232CAN);
            if (gList.Count < 1)
            {
                log.Error("!!! Can Not Find CAN Gates !!!");
                return;
            }
            foreach (CanGate canGate in gList)
            {
                log.Info("--> Find Can Gate: " + canGate.info.canName + "startNodeAddr:" + canGate.info.startNodeAddr);
                foreach (DictionaryEntry en in canGateInfoMap)//配置文件中的信息
                {
                    CanGate.CanGateInfo info = (CanGate.CanGateInfo)en.Value;
                    if (info.startNodeAddr == canGate.info.startNodeAddr)
                    {
                        log.Info("<-- startNode Addr Matched at Can Gate: " + canGate.info.canName + "startNodeAddr:" + canGate.info.startNodeAddr);
                        canGate.info.canName = info.canName;
                        canGate.info.canType = info.canType;
                        canGate.info.expectNodeNum = info.expectNodeNum;
                        info.gateType = canGate.info.gateType;
                        info.canIndex = canGate.info.canIndex;
                        canGateMap.Add(canGate.info.canName, canGate);//add to the actual Matrix Map
                    }
                }
            }
            log.Info("==============match canGate done ==============");
            foreach (DictionaryEntry rowT in matrix)
            {
                Hashtable rowTable = (Hashtable)rowT.Value;
                foreach (DictionaryEntry colTable in rowTable)
                {
                    StepperMotor motor = (StepperMotor)colTable.Value;
                    foreach (DictionaryEntry en in canGateMap)
                    {
                        CanGate canGate = (CanGate)en.Value;
                        if (motor.gateName.Equals(canGate.info.canName))/////////!!!canGate的名字也要和配置文件匹配
                        {
                            motor.gateAddr = canGate.info.canIndex;
                        }
                    }

                    foreach (CanGate canGate in gList)
                    {
                        if (canGate.nodeAddrs.Contains(motor.nodeAddr) 
                            && motor.gateAddr == canGate.info.canIndex
                            && motor.gateName.Equals(canGate.info.canName))
                        {
                            /// node 存在 且 网关存在
                            motor.foundInDriver = true;
                            log.Debug("found motor, gateAddr: " + motor.gateAddr +
                                        ", canGate.info.canIndex: " + canGate.info.canIndex +
                                        ", gateName: " + motor.gateName + ", canGate.info.canName: " +
                                        canGate.info.canName);
                        }
                        else
                        {
                            Console.WriteLine("notfound motor, gateAddr: " + motor.gateAddr +
                                ", canGate.info.canIndex: " + canGate.info.canIndex +
                                ", gateName: " + motor.gateName + ", canGate.info.canName: " +
                                canGate.info.canName);
                            log.Warn("notfound motor, gateAddr: " + motor.gateAddr +
                                ", canGate.info.canIndex: " + canGate.info.canIndex +
                                ", gateName: " + motor.gateName + ", canGate.info.canName: " +
                                canGate.info.canName);
                        }
                    }
                }
            }
        }

        bool readCanGateArray(ref JsonReader reader)
        { 
            while (reader.Read())   // gate object
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    if(false == readCanGate(ref reader))
                        return false;
                } 
                else if (reader.TokenType == JsonToken.EndArray)                
                    break;       
            }
            return true;
        }

        bool  readCanGate(ref JsonReader reader)
        {
            // create new canGateInfo            
            CanGate.CanGateInfo canGateInfo = new CanGate.CanGateInfo() { 
                canIndex = -1,
                startNodeAddr = -1,
                expectNodeNum = -1,
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                    break;                                   
                else if (reader.TokenType == JsonToken.PropertyName && "type".Equals((string)reader.Value))
                {
                    reader.Read();                    
                    canGateInfo.canType = (string)reader.Value;
                }
                else if (reader.TokenType == JsonToken.PropertyName && "gateName".Equals((string)reader.Value))
                {
                    reader.Read();                    
                    canGateInfo.canName = (string)reader.Value;
                }else if(reader.TokenType == JsonToken.PropertyName && "expectNodeNum".Equals((string)reader.Value))
                {
                    reader.Read();                    
                    canGateInfo.expectNodeNum = Convert.ToInt32(reader.Value);
                }
                else if (reader.TokenType == JsonToken.PropertyName && "startNodeAddr".Equals((string)reader.Value))
                {
                    reader.Read();                    
                    canGateInfo.startNodeAddr = Convert.ToInt32(reader.Value);
                }
                else if (reader.TokenType == JsonToken.PropertyName && "nodes".Equals((string)reader.Value))
                {
                    reader.Read();
                    if (reader.TokenType != JsonToken.StartArray)
                    {
                        Console.WriteLine("nodes must start as an Array.");
                        return false;
                    }
                    else if (readNodeArray(ref reader, ref canGateInfo) == false)
                    {
                        return false;
                    }
                }
            }
                        
            canGateInfoMap.Add(canGateInfo.canName, canGateInfo);
            return true;
        }

        bool readNodeArray(ref JsonReader reader, ref CanGate.CanGateInfo canGateInfo)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    if (readNode(ref reader, ref canGateInfo) == false)
                    {
                        return false;
                    }
                }

                else if(reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }
            }
            return true;
        }

        bool readNode(ref JsonReader reader, ref CanGate.CanGateInfo canGateInfo)
        {
            StepperMotor sm = new StepperMotor() { 
                foundInDriver = false,
            };
            sm.gateName = canGateInfo.canName;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && "nodeAddr".Equals((string)reader.Value))
                {
                    reader.Read();
                    sm.nodeAddr = Convert.ToInt32(reader.Value);
                }
                else if (reader.TokenType == JsonToken.PropertyName && "row".Equals((string)reader.Value))
                {
                    reader.Read();
                    sm.row = Convert.ToInt32(reader.Value);
                    
                }
                else if (reader.TokenType == JsonToken.PropertyName && "col".Equals((string)reader.Value))
                {
                    reader.Read();
                    sm.col = Convert.ToInt32(reader.Value);
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            {
                if(sm.row < 0 || sm.col <0 ){
                    Console.WriteLine("Wrong node format.");
                    return false;
                }

                if (canGateInfo.startNodeAddr > sm.nodeAddr || canGateInfo.startNodeAddr == -1)
                {
                    canGateInfo.startNodeAddr = sm.nodeAddr;
                }

                if (matrix.ContainsKey(sm.row))
                {
                    Hashtable rowTable = (Hashtable)matrix[sm.row];
                    rowTable.Add(sm.col, sm);
                }
                else
                {
                    Hashtable rowTable = new Hashtable();
                    rowTable.Add(sm.col, sm);
                    matrix.Add(sm.row, rowTable);
                }
            }
            
            return true;
        }
        

        private MotorMatrix()
        {

        }



        public void enableMotorsAll()
        {
            foreach (int k in matrix.Keys)
            {
                enableMotorsRow(k);
            }
            {
                // 因为装置硬件原因，第一行不允许旋转
                disableMotorsRow(1);
            }
        }

        public void enableMotorsRow(int row)
        {
            {
                // 因为装置硬件原因，第一行不允许旋转
                if (row == 1) return;
            }
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                ((StepperMotor)en.Value).enable();
            }
        }
        public void enableMotor(int row, int col)
        {
            StepperMotor m = getStepperMotro(row, col);
            if(m == null) return;
            m.enable();
        }

        public void disableMotorsAll()
        {
            foreach (int k in matrix.Keys)
            {
                disableMotorsRow(k);
            }
        }

        public void disableMotorsRow(int row)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                ((StepperMotor)en.Value).disable();
            }
        }

        public void disableMotor(int row, int col)
        {
            StepperMotor m = getStepperMotro(row, col);
            if(m == null) return;
            m.disable();
        }

        public StepperMotor getStepperMotro(int row, int col)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return null;
            return (StepperMotor)rowTable[col];
        }

        public void setSpeedMotorsAll(int speedValue)
        {
            foreach (int k in matrix.Keys)
            {
                setSpeedMotorsRow(k, speedValue);
            }
        }

        public void setSpeedMotorsRow(int row, int speedValue)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                ((StepperMotor)en.Value).setSpeed(speedValue);
            }
        }

        public void setSpeedMotor(int row, int col, int speedValue)
        {
            StepperMotor m = getStepperMotro(row, col);
            if(m == null) return;
            m.setSpeed(speedValue);
        }

        public void setAccMotorsAll(int accValue)
        {
            foreach (int k in matrix.Keys)
            {
                setAccMotorsRow(k, accValue);
            }
        }

        public void setAccMotorsRow(int row, int accValue)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                ((StepperMotor)en.Value).setSpeed(accValue);
            }
        }

        public void setAccMotor(int row, int col, int accValue)
        {
            StepperMotor m = getStepperMotro(row, col);
            if(m == null) return;
            m.setAcc(accValue);
        }

        public void enableSignalStopAll()
        {
            foreach (int k in matrix.Keys)
            {
                enableSignalStopRow(k);
            }
        }

        public void enableSignalStopRow(int row)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                ((StepperMotor)en.Value).enableSignalStop();
            }
        }

        public void enableSignaleStop(int row, int col)
        {
            StepperMotor m = getStepperMotro(row, col);
            if(m == null) return;
            m.enableSignalStop();
        }

        public void disableSignalStopAll()
        {
            foreach (int k in matrix.Keys)
            {
                disableSignalStopRow(k);
            }
        }

        public void disableSignalStopRow(int row)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                ((StepperMotor)en.Value).disableSignalStop();
            }
        }

        public void disableSignaleStop(int row, int col)
        {
            StepperMotor m = getStepperMotro(row, col);
            if(m == null) return;
            m.disableSignalStop();
        }

        const int defaultWaitTime = 500;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="speedValue"></param>
        /// <param name="time"> in micro seconds </param>
        public void waitDesiredSpeedAll(int speedValue, int time = defaultWaitTime)
        {
            foreach (int k in matrix.Keys)
            {
                waitDesiredSpeedRow(k, speedValue, time);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="speedValue"></param>
        /// <param name="time">in micro seconds</param>
        public void waitDesiredSpeedRow(int row, int speedValue, int time = defaultWaitTime)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                ((StepperMotor)en.Value).waitToDesiredSpeed(speedValue, defaultWaitTime);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="speedValue"></param>
        /// <param name="time">micro seconds</param>
        public void waitDesiredSpeed(int row, int col, int speedValue, int time = defaultWaitTime)
        {
            StepperMotor m = getStepperMotro(row, col);
            if(m == null) return;
            m.waitToDesiredSpeed(speedValue, defaultWaitTime);
        }

        public void printMatrix(string fname)
        {           
            StreamWriter log = new StreamWriter(fname);            
            foreach (int k in matrix.Keys)
            {
                foreach (DictionaryEntry en in (Hashtable)matrix[k])
                {
                    StepperMotor motor = ((StepperMotor)en.Value);
                    string str = string.Format("row:{0}, col:{1}, gateAddr:{2}, nodeAddr:{3}, foundInDriver:{4}",
                        k, en.Key, motor.gateAddr, motor.nodeAddr, motor.foundInDriver);
                    str += "\r\n";
                    log.WriteLine(str);
                    
                }
            }
            log.Flush();
        }

        public void printUnFindMotor(string fname)
        {           
            StreamWriter log = new StreamWriter(fname);
            foreach (int k in matrix.Keys)
            {
                foreach (DictionaryEntry en in (Hashtable)matrix[k])
                {
                    StepperMotor motor = ((StepperMotor)en.Value);
                    if (motor.foundInDriver == false)
                    {
                        string str = string.Format("row:{0}, col:{1}, gateAddr:{2}, nodeAddr:{3}, foundInDriver:{4}",
                            k, en.Key, motor.gateAddr, motor.nodeAddr, motor.foundInDriver);
                        str += "\r\n";
                        log.WriteLine(str);
                    }

                }
            }
            log.Flush();
        }

        public delegate void OperationOnStepMotor(StepperMotor motor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        public void employOperationOnMotorsAll(OperationOnStepMotor op)
        {
            foreach (int k in matrix.Keys)
            {
                employOperationOnMotorsRow(k, op);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="op"></param>
        public void employOperationOnMotorsRow(int row, OperationOnStepMotor op)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                StepperMotor m = ((StepperMotor)en.Value);                
                op(m);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="op"></param>
        public void employOperationOnMotor(int row, int col, OperationOnStepMotor op)
        {
            StepperMotor m = getStepperMotro(row, col); 
            if(m == null)return;
            op(m);
        }


        public void speedDeltaAll(int delta)
        {
            foreach (int k in matrix.Keys)
            {
                speedDeltaRow(k, delta);
            }
        }

        public void speedDeltaRow(int row, int delta)
        {
            Hashtable rowTable = (Hashtable)matrix[row];
            if (rowTable == null) return;
            foreach (DictionaryEntry en in rowTable)
            {
                if (delta < 0)
                {
                    ((StepperMotor)en.Value).speedDown(Math.Abs(delta));
                }
                else if (delta > 0)
                {
                    ((StepperMotor)en.Value).speedUp(Math.Abs(delta));
                }
                else
                {

                }
            }
                
        }

        public void speedDelta( int row, int col, int delta)
        {
            StepperMotor m = getStepperMotro(row, col); 
            if(m == null) return;
            if (delta < 0)
            {
                m.speedDown(Math.Abs(delta));   
            }
            else if (delta > 0)
            {
                m.speedUp(Math.Abs(delta));
            }
            else
            {

            }
        }
    }
}
