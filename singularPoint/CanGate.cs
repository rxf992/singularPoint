using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace canGate
{
    using System.IO.Ports;
    using Microsoft.Win32;
    using UIM512Driver;

    class CanGate
    {
        public const int THIRD_PARTY_DEV = 0x0;
        public const int UIDEV_ALL = 0x10;
        public const int UIDEV_RS232CAN = 0x20;
        public const int UIDEV_PCICAN = 0x40;
        public const int UIDEV_USBCAN = 0x60;
        public const int UIDEV_ETHCAN = 0x80;

        public List<int> nodeAddrs = new List<int>();

        public struct CanGateInfo
        {
            public int gateType;

            /// <summary>
            /// CAN 总线设备索引号
            /// </summary>
            public int canIndex;
            
            /// <summary>
            /// 此网关下总线上最低地址的起始地址值
            /// </summary>
            public int startNodeAddr; 

            public string canType;
            public string canName;
            public int expectNodeNum;
        }

        public CanGateInfo info;               

        public struct USBSerialDriver
        {
            String uniqueStr;
            int index;
        }

        private CanGate()
        {

        }

        static public CanGate create()
        {
            CanGate instance = new CanGate();
            return instance;
        }


        static public int getGateIndexWithStartNodeIndex(int startNodeIndex)
        {
            int index = 0;
            return index;
        }
                
        static public List<CanGate> getGateList(int gateType)
        {
            List<CanGate> gList = new List<CanGate>();
            switch (gateType)
            {
                case UIDEV_RS232CAN:
                    {
                        int totalGateNum = UIM512Driver.SearchDevice(UIDEV_RS232CAN);
                        int[] gateAddrList = new int[totalGateNum];
                        UIM512Driver.GetUimDevIdList(UIDEV_RS232CAN, gateAddrList);
                        for (int i = 0; i < totalGateNum; i++)
                        {
                            CanGate gate = new CanGate();                            
                            gate.info.gateType = UIDEV_RS232CAN;
                            gate.info.canIndex = gateAddrList[i];
                            
                            // 注册网关
                            if (1 == UIM512Driver.OpenUimDev(gate.info.canIndex))
                            {                                
                                //第一次调用全局注册函数，获取CAN结点数量
                                int tmpNodeNum = UIM512Driver.UimGrobReg(gate.info.canIndex, null);
                                int[] canNodeAddrList = new int[tmpNodeNum];
                                UIM512Driver.UimGrobReg(gate.info.canIndex, canNodeAddrList);

                                for (int j = 0; j < tmpNodeNum; j++)
                                {
                                    gate.nodeAddrs.Add(canNodeAddrList[j]);
                                }

                                if (gate.nodeAddrs.Count > 0)
                                {
                                    gate.nodeAddrs = gate.nodeAddrs.OrderBy(s => s).ToList();
                                    gate.info.startNodeAddr = gate.nodeAddrs[0];
                                }
                            }
                            gList.Add(gate);
                        }

                    } break;
                default: break;
            }
            return gList;
        }

    }
}
