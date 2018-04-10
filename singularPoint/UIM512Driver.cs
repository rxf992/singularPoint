using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace UIM512Driver
{
    public struct CAN_MSG_OBJ
    {
        public int ID;                    //报文 ID  = SID(11位) | EID (18位)
        public int Reserved0;         //保留, 赋值0
        public byte Reserved1;         //保留, 赋值0
        public byte SendType;		    //0：正常发送，1：自发自收
        public byte IDE;	                //0：标准帧，  1：扩展帧
        public byte RTR;	                //0：数据帧，  1：远程帧
        public byte DataLen;		    //表明Data[8]数组内的的字节数，长度不能超过8；
        //CAN数据包原为8个字节，为了支持RS232，此数据的长度增加为128
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Data;
        //系统保留
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved;
    };


    public struct DEV_INFO_OBJ
    {
        public uint dwDevType;		        //0x11代表RS232CAN 0x20代表PCICAN 
        public uint dwDevIndex;		        //软件启动时为设备分配的ID
        public uint uiComIndex;               //系统分配的COM编号
        public uint uiBaudRate;				//COM口对应的波特率
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string pszDevName;  //设备型号和名字
        public uint Protocol;                 //是字符串传输还是数据传输
    }

    //电机型号和固件号信息
    public struct MDL_INFO_OBJ
    {
        public uint uiCANNodeID;					//驱动器标识码
        public uint uiCANNodeType;				//驱动器型号
        public uint uiCurrent;						//电流
        public bool bIntegrationEncode;		//内置编码器
        public bool bEnCode;						//闭环控制
        public bool bMotion;						//高级运动控制
        public bool b2Sensor;						//"-SP"
        public bool b4Sensor;						//"-S"
        public uint uiFirewareVersion;			//驱动器固件版本
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string szModelName;		//驱动器型号
    }


    //Basic Instruction Acknowledgment
    public struct BASIC_ACK_OBJ
    {
        public uint uiReserv;         //0
        public bool bENA;             //电机使能状态
        public bool bDIR;             //电机方向
        public bool bACR;             //电流减半
        public uint uiMCS;             //电机细分
        public uint uiCUR;             //电流
        public uint uiSPD;             //当前速度
        public uint uiSTP;             //当前步长
    };

    //Basic Instruction Feedback
    public struct BASIC_FBK_OBJ
    {
        public uint uiReserv;         //0
        public bool bENA;             //电机使能状态
        public bool bDIR;             //电机方向
        public bool bACR;             //电流减半
        public uint uiMCS;             //电机细分
        public uint uiCUR;             //电流
        public uint uiSPD;             //当前速度
        public uint uiSTP;             //当前步长
    };

    public struct P_S12CON
    {
        public uint uiS2RACT;
        public uint uiS2FACT;
        public uint uiS1RACT;
        public uint uiS1FACT;
    };

    public struct UIM_MCFG_INFO_OBJ
    {
        public uint uiMcfgVal;
    };

    class UIM512Driver
    {

        public static int fSearchDevice(int dwDevType)
        {
            return SearchDevice(dwDevType);
        }
        //函数导入
        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SearchDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SearchDevice(int dwDevType);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SearchGateway", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SearchGateway(int dwDevType, DEV_INFO_OBJ[] objs, int obj_num);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetUimDevIdList", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetUimDevIdList(int dwDevType, Int32[] pDevIndexList);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetUimDevInfo", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetUimDevInfo(int dwDevType, ref DEV_INFO_OBJ devInfoObj);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "OpenUimDev", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenUimDev(int dwDevType);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "CloseUimDev", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseUimDev(int dwDevType);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "UimGrobReg", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int UimGrobReg(int dwDevType, Int32[] pCanNodeIdList);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetMDL", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMDL(int dwDevType, int dwCanNodeId, ref MDL_INFO_OBJ pMDLInfoObj);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "UimENA", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int UimENA(int dwDevType, int dwCanNodeId, bool bAckEna, ref BASIC_ACK_OBJ pBasicAckobj);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "UimOFF", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int UimOFF(int dwDevType, int dwCanNodeId, bool bAckEna, ref BASIC_ACK_OBJ pBasicAckobj);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetORG", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetORG(int dwDevType, int dwCanNodeId, int iValue, bool bAckEna, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "UimFBK", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int UimFBK(int dwDevType, int dwCanNodeId, ref BASIC_FBK_OBJ pFBKInfo);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetSPD", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSPD(int dwDevType, int dwCanNodeId, int iValue, bool bAckEna, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetmACC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetmACC(int dwDevIndex, int dwCanNodeId, int iValue, bool bAckEna, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetSTP", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSTP(int dwDevType, int dwCanNodeId, int iValue, bool bAckEna, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetPOS", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPOS(int dwDevType, int dwCanNodeId, int iValue, bool bAckEna, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetQEC", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetQEC(int dwDevType, int dwCanNodeId, int iValue, bool bAckEna, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetSPD", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSPD(int dwDevType, int dwCanNodeId, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetmACC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetmACC(int dwDevType, int dwCanNodeId, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetSTP", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSTP(int dwDevType, int dwCanNodeId, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetPOS", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetPOS(int dwDevType, int dwCanNodeId, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetQEC", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetQEC(int dwDevType, int dwCanNodeId, ref int pRtnValue);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetUimMCFG", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetUimMCFG(int dwDevType, int dwCanNodeId, ref UIM_MCFG_INFO_OBJ pUIM_MCFG_INFO_OBJ_IN, bool bAckEna, ref UIM_MCFG_INFO_OBJ pUIM_MCFG_INFO_OBJ_OUT);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "GetUimMCFG", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetUimMCFG(int dwDevType, int dwCanNodeId, ref UIM_MCFG_INFO_OBJ pUIM_MCFG_INFO_OBJ_OUT);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetS12CON", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetS12CON(int dwDevType, int dwCanNodeId, ref P_S12CON pS12CON_IN, bool bAckEna, ref P_S12CON pS12CON_OUT);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "SetS34CON", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetS34CON(int dwDevType, int dwCanNodeId, ref P_S12CON pS34CON_IN, bool bAckEna, ref P_S12CON pS34CON_OUT);

        [DllImport("..\\DLL\\UISimCanFunc.dll", EntryPoint = "UIMRegRtcnCallBack", CharSet = CharSet.Ansi,  CallingConvention = CallingConvention.Cdecl)]
        public static extern int UIMRegRtcnCallBack(int dwDevType, int dwDevIndex, ProcessDelegate pFunc);

        public delegate void ProcessDelegate(int dwDevIndex, ref  CAN_MSG_OBJ can_msg_obj, int dwMsgLen);
        public ProcessDelegate m_delRtcnProcess = null;
    
    }
}
