using System;
using System.Runtime.InteropServices;

namespace Gamemaker.Internal
{
    public enum EGmsEventType
    {
        EVENT_ASYNC_STEAM = 69,
        EVENT_ASYNC_SOCIAL = 70
    }

    public static class GamemakerInternal
    {
        internal delegate void GmlEventPerformAsyncDelegate(int mapID, int eventType);
        internal delegate int GmlDsMapCreateDelegate(int n);
        internal delegate bool GmlDsMapAddDoubleDelegate(int mapID, string key, double value);
        internal delegate bool GmlDsMapAddStringDelegate(int mapID, string key, string value);

        internal static GmlEventPerformAsyncDelegate GmlEventPerformAsync_Internal;
        internal static GmlDsMapCreateDelegate GmlDsMapCreate_Internal;
        internal static GmlDsMapAddDoubleDelegate GmlDsMapAddDouble_Internal;
        internal static GmlDsMapAddStringDelegate GmlDsMapAddString_Internal;

        [DllExport("RegisterCallbacks", CallingConvention.Cdecl)]
        public static unsafe double RegisterCallbacks(IntPtr ptr1, IntPtr ptr2, IntPtr ptr3, IntPtr ptr4)
        {
            GmlEventPerformAsync_Internal = Marshal.GetDelegateForFunctionPointer<GmlEventPerformAsyncDelegate>(ptr1);
            GmlDsMapCreate_Internal = Marshal.GetDelegateForFunctionPointer<GmlDsMapCreateDelegate>(ptr2);
            GmlDsMapAddDouble_Internal = Marshal.GetDelegateForFunctionPointer<GmlDsMapAddDoubleDelegate>(ptr3);
            GmlDsMapAddString_Internal = Marshal.GetDelegateForFunctionPointer<GmlDsMapAddStringDelegate>(ptr4);

            return 1;
        }
    }
    public class DsMap
    {
        public static string TypeKey { get; set; } = "_type_";

        public int ID { get; private set; } = -1;

        public DsMap()
        {
            ID = GamemakerInternal.GmlDsMapCreate_Internal(0);
        }

        public DsMap(int typeID)
        {
            ID = GamemakerInternal.GmlDsMapCreate_Internal(0);
            AddKeyValuePair(TypeKey, typeID);
        }

        public static implicit operator double(DsMap map)
        {
            return map.ID;
        }

        public object this[string key]
        {
            set
            {
                if(
                    value is double ||
                    value is float  ||
                    value is decimal  ||
                    value is sbyte ||
                    value is byte ||
                    value is short ||
                    value is ushort ||
                    value is int ||
                    value is uint
                )
                    AddKeyValuePair(key, (double)Convert.ChangeType(value, typeof(double)));
                else if (value is bool)
                    AddKeyValuePair(key, (bool)value ? 1d : 0d);
                else if (value is DsMap)
                    AddKeyValuePair(key, (DsMap)value);//nested maps
                else if (value is null)
                    AddKeyValuePair(key, "");//empty str instead of null
                else//+long, ulong
                    AddKeyValuePair(key, value.ToString());//we can assign objects directly that implement custom ToString overload (like json objects and etc)
            }
        }

        public bool AddKeyValuePair(string key, double value)
        {
            if (ID != -1)
            {
                GamemakerInternal.GmlDsMapAddDouble_Internal(ID, key, value);
                return true;
            }

            return false;
        }

        public bool AddKeyValuePair(string key, string value)
        {
            if (ID != -1)
            {
                GamemakerInternal.GmlDsMapAddString_Internal(ID, key, value);
                return true;
            }

            return false;
        }

        public bool Dispatch(EGmsEventType eventType)
        {
            if (ID != -1)
            {
                GamemakerInternal.GmlEventPerformAsync_Internal(ID, (int)eventType);

                return true;
            }

            return false;
        }

        public bool Dispatch(string typeName, EGmsEventType eventType)
        {
            //return AddKeyValuePair(TypeKey, typeName) && Dispatch(eventType);

            if (ID != -1)
            {
                AddKeyValuePair(TypeKey, typeName);

                GamemakerInternal.GmlEventPerformAsync_Internal(ID, (int)eventType);

                return true;
            }

            return false;
        }

        public bool Dispatch(int typeID, EGmsEventType eventType)
        {
            if (ID != -1)
            {
                AddKeyValuePair(TypeKey, typeID);

                GamemakerInternal.GmlEventPerformAsync_Internal(ID, (int)eventType);

                return true;
            }

            return false;
        }
    }
}
