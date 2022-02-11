using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TelloDroneControl
{
    public sealed class Joystickxx
    {
        public static List<Joystick> GetAllJoysticks()
        {
            List<Joystick> result = new List<Joystick>();
            var count = GetJoystickCount();
            for (int i = 0; i <= count - 1; i++)
                try
                {
                    result.Add(new(i));
                }
                catch { }
            return result;
        }

        public int ID { get; }
        public JoystickInfo Info { get; }
        public string Name { get => Info.szPname; }

        public Joystick(int id)
        {
            this.ID = id;
            JoystickInfo info = new JoystickInfo();
            MMSYSERR result = GetJostickInfoInternal(id, ref info, Marshal.SizeOf(info));
            if (result != MMSYSERR.MMSYSERR_NOERROR)
                throw new Exception($"Joystick Exception: {result}\r\nIs the Joystick pluged in?!");
            this.Info = info;
        }

        /// <summary>
        /// Value between -100 and 100
        /// </summary>
        /// <returns></returns>
        public double X
        {
            get
            {
                var data = GetRawData();
                Console.WriteLine($"{data.dwXpos} {data.dwYpos} {data.dwZpos} {data.dwRpos}");
                return (data.dwXpos - (Info.wXmax / (double)2.0F)) / (double)(Info.wXmax / (double)2.0F) * 100.0F;
            }
        }
        public double Y
        {
            get
            {
                var data = GetRawData();
                return (data.dwYpos - (Info.wYmax / (double)2.0F)) / (double)(Info.wYmax / (double)2.0F) * 100.0F;
            }
        }

        public double X2
        {
            get
            {
                var data = GetRawData();
                return (data.dwUpos - (Info.wUmax / (double)2.0F)) / (double)(Info.wUmax / (double)2.0F) * 100.0F;
            }
        }
        public double Y2
        {
            get
            {
                var data = GetRawData();
                return (data.dwRpos - (Info.wRmax / (double)2.0F)) / (double)(Info.wRmax / (double)2.0F) * 100.0F;
            }
        }

        public JOY_POV LookAroundButton
        {
            get
            {
                var data = GetRawData();
                return data.dwPOV;
            }
        }

        public JoystickDataEx GetRawData()
        {
            JoystickDataEx result = new();
            result.dwFlags = JOYFlags.JOY_RETURNALL;
            result.dwSize = Marshal.SizeOf<JoystickDataEx>();
            var error = GetJostickDataInternal(ID, ref result);
            if (error != MMSYSERR.MMSYSERR_NOERROR)
                throw new Exception(error.ToString());
            return result;
        }

        /// <summary>
        /// JOYCAPS structure
        /// https://docs.microsoft.com/en-us/previous-versions/dd757103(v=vs.85)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct JoystickInfo
        {
            public ushort wMid;
            public ushort wPid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint wXmin;
            public uint wXmax;
            public uint wYmin;
            public uint wYmax;
            public uint wZmin;
            public uint wZmax;
            public uint wNumButtons;
            public uint wPeriodMin;
            public uint wPeriodMax;
            public uint wRmin;
            public uint wRmax;
            public uint wUmin;
            public uint wUmax;
            public uint wVmin;
            public uint wVmax;
            public uint wCaps;
            public uint wMaxAxes;
            public uint wNumAxes;
            public uint wMaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szRegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szOEMVxD;
        }

        /// <summary>
        /// JOYINFOEX structure
        /// https://docs.microsoft.com/en-us/previous-versions/ms709358(v=vs.85)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct JoystickDataEx
        {
            public int dwSize;
            public JOYFlags dwFlags;
            public int dwXpos;
            public int dwYpos;
            public int dwZpos;
            public int dwRpos;
            public int dwUpos;
            public int dwVpos;
            public JOYButtons dwButtons;
            public int dwButtonNumber;
            public JOY_POV dwPOV;
            public int dwReserved1;
            public int dwReserved2;
        }

        [Flags]
        public enum JOYFlags
        {
            JOY_RETURNX = 0x1,
            JOY_RETURNY = 0x2,
            JOY_RETURNZ = 0x4,
            JOY_RETURNR = 0x8,
            JOY_RETURNU = 0x10,
            JOY_RETURNV = 0x20,
            JOY_RETURNPOV = 0x40,
            JOY_RETURNBUTTONS = 0x80,
            JOY_RETURNRAWDATA = 0x100,
            JOY_RETURNPOVCTS = 0x200,
            JOY_RETURNCENTERED = 0x400,
            JOY_USEDEADZONE = 0x800,
            JOY_RETURNALL = (JOYFlags.JOY_RETURNX | JOYFlags.JOY_RETURNY | JOYFlags.JOY_RETURNZ | JOYFlags.JOY_RETURNR | JOYFlags.JOY_RETURNU | JOYFlags.JOY_RETURNV | JOYFlags.JOY_RETURNPOV | JOYFlags.JOY_RETURNBUTTONS)
        }

        [Flags]
        public enum JOYButtons
        {
            JOY_BUTTON1 = 0x1,
            JOY_BUTTON2 = 0x2,
            JOY_BUTTON3 = 0x4,
            JOY_BUTTON4 = 0x8,
            JOY_BUTTON1CHG = 0x100,
            JOY_BUTTON2CHG = 0x200,
            JOY_BUTTON3CHG = 0x400,
            JOY_BUTTON4CHG = 0x800,
            JOY_BUTTON5 = 0x10,
            JOY_BUTTON6 = 0x20,
            JOY_BUTTON7 = 0x40,
            JOY_BUTTON8 = 0x80,
            JOY_BUTTON9 = 0x100,
            JOY_BUTTON10 = 0x200,
            JOY_BUTTON11 = 0x400,
            JOY_BUTTON12 = 0x800,
            JOY_BUTTON13 = 0x1000,
            JOY_BUTTON14 = 0x2000,
            JOY_BUTTON15 = 0x4000,
            JOY_BUTTON16 = 0x8000,
            JOY_BUTTON17 = 0x10000,
            JOY_BUTTON18 = 0x20000,
            JOY_BUTTON19 = 0x40000,
            JOY_BUTTON20 = 0x80000,
            JOY_BUTTON21 = 0x100000,
            JOY_BUTTON22 = 0x200000,
            JOY_BUTTON23 = 0x400000,
            JOY_BUTTON24 = 0x800000,
            JOY_BUTTON25 = 0x1000000,
            JOY_BUTTON26 = 0x2000000,
            JOY_BUTTON27 = 0x4000000,
            JOY_BUTTON28 = 0x8000000,
            JOY_BUTTON29 = 0x10000000,
            JOY_BUTTON30 = 0x20000000,
            JOY_BUTTON31 = 0x40000000
        }

        public enum JOY_POV
        {
            JOY_POVCENTERED = -1,
            JOY_POVFORWARD = 0,
            JOY_POVRIGHT = 9000,
            JOY_POVBACKWARD = 18000,
            JOY_POVLEFT = 27000
        }

        private enum MMSYSERR
        {
            MMSYSERR_NOERROR = 0,
            MMSYSERR_ERROR = (MMSYSERR.MMSYSERR_NOERROR + 1),
            MMSYSERR_BADDEVICEID = (MMSYSERR.MMSYSERR_NOERROR + 2),
            MMSYSERR_NOTENABLED = (MMSYSERR.MMSYSERR_NOERROR + 3),
            MMSYSERR_ALLOCATED = (MMSYSERR.MMSYSERR_NOERROR + 4),
            MMSYSERR_INVALHANDLE = (MMSYSERR.MMSYSERR_NOERROR + 5),
            MMSYSERR_NODRIVER = (MMSYSERR.MMSYSERR_NOERROR + 6),
            MMSYSERR_NOMEM = (MMSYSERR.MMSYSERR_NOERROR + 7),
            MMSYSERR_NOTSUPPORTED = (MMSYSERR.MMSYSERR_NOERROR + 8),
            MMSYSERR_BADERRNUM = (MMSYSERR.MMSYSERR_NOERROR + 9),
            MMSYSERR_INVALFLAG = (MMSYSERR.MMSYSERR_NOERROR + 10),
            MMSYSERR_INVALPARAM = (MMSYSERR.MMSYSERR_NOERROR + 11),
            MMSYSERR_HANDLEBUSY = (MMSYSERR.MMSYSERR_NOERROR + 12),
            MMSYSERR_INVALIDALIAS = (MMSYSERR.MMSYSERR_NOERROR + 13),
            MMSYSERR_BADDB = (MMSYSERR.MMSYSERR_NOERROR + 14),
            MMSYSERR_KEYNOTFOUND = (MMSYSERR.MMSYSERR_NOERROR + 15),
            MMSYSERR_READERROR = (MMSYSERR.MMSYSERR_NOERROR + 16),
            MMSYSERR_WRITEERROR = (MMSYSERR.MMSYSERR_NOERROR + 17),
            MMSYSERR_DELETEERROR = (MMSYSERR.MMSYSERR_NOERROR + 18),
            MMSYSERR_VALNOTFOUND = (MMSYSERR.MMSYSERR_NOERROR + 19),
            MMSYSERR_NODRIVERCB = (MMSYSERR.MMSYSERR_NOERROR + 20),
            MMSYSERR_MOREDATA = (MMSYSERR.MMSYSERR_NOERROR + 21),
            MMSYSERR_LASTERROR = (MMSYSERR.MMSYSERR_NOERROR + 21)
        }

        [DllImport("Winmm.dll", EntryPoint = "joyGetDevCaps", SetLastError = true), PreserveSig]
        private extern static MMSYSERR GetJostickInfoInternal(int uJoyID, ref JoystickInfo pjc, int cbjc);

        [DllImport("Winmm.dll", EntryPoint = "joyGetPosEx", SetLastError = true), PreserveSig]
        private extern static MMSYSERR GetJostickDataInternal(int uJoyID, ref JoystickDataEx pji);

        [DllImport("Winmm.dll", EntryPoint = "joyGetNumDevs", SetLastError = true), PreserveSig]
        private extern static int GetJoystickCount();
    }

}
