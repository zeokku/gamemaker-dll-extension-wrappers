using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Gamemaker
{
    public static class Internal
    {
        enum EventIDs
        {
            EVENT_OTHER_SOCIAL = 70
        }

        private delegate void GmlEventPerformAsyncDelegate(int mapID, int eventType = (int)EventIDs.EVENT_OTHER_SOCIAL);
        private delegate int GmlDsMapCreateDelegate(int n);
        private delegate bool GmlDsMapAddDoubleDelegate(int mapID, string key, double value);
        private delegate bool GmlDsMapAddStringDelegate(int mapID, string key, string value);

        private static GmlEventPerformAsyncDelegate GmlEventPerformAsync_Internal;
        private static GmlDsMapCreateDelegate GmlDsMapCreate_Internal;
        private static GmlDsMapAddDoubleDelegate GmlDsMapAddDouble_Internal;
        private static GmlDsMapAddStringDelegate GmlDsMapAddString_Internal;

        [DllExport("RegisterCallbacks", CallingConvention.Cdecl)]
        public static unsafe double RegisterCallbacks(IntPtr ptr1, IntPtr ptr2, IntPtr ptr3, IntPtr ptr4)
        {
            GmlEventPerformAsync_Internal = Marshal.GetDelegateForFunctionPointer<GmlEventPerformAsyncDelegate>(ptr1);
            GmlDsMapCreate_Internal = Marshal.GetDelegateForFunctionPointer<GmlDsMapCreateDelegate>(ptr2);
            GmlDsMapAddDouble_Internal = Marshal.GetDelegateForFunctionPointer<GmlDsMapAddDoubleDelegate>(ptr3);
            GmlDsMapAddString_Internal = Marshal.GetDelegateForFunctionPointer<GmlDsMapAddStringDelegate>(ptr4);

            return 1;
        }

        public class DsMap
        {
            public int ID { get; private set; } = -1;

            public DsMap()
            {
                ID = GmlDsMapCreate_Internal(0);
            }

            public object this[string key]
            {
                set
                {
                    if(
                        value is double 
                        || value is float 
                        || value is decimal 
                        || value is sbyte
                        || value is byte
                        || value is short
                        || value is ushort
                        || value is int
                        || value is uint
                    )
                        AddKeyValuePair(key, (double)value);
                    else if(value is bool)
                        AddKeyValuePair(key, (bool)value ? 1d : 0d);
                    else
                        AddKeyValuePair(key, value.ToString());//we can assign objects directly that implement custom ToString overload (like json objects and etc)
                }
            }

            public bool AddKeyValuePair(string key, double value)
            {
                if (ID != -1)
                {
                    GmlDsMapAddDouble_Internal(ID, key, value);
                    return true;
                }

                return false;
            }

            public bool AddKeyValuePair(string key, string value)
            {
                if (ID != -1)
                {
                    GmlDsMapAddString_Internal(ID, key, value);
                    return true;
                }

                return false;
            }

            public bool Dispatch(string type = null)
            {
                if(type != null) AddKeyValuePair("_type", type);

                GmlEventPerformAsync_Internal(ID);

                return true;
            }
        }
    }
}
