using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace CompatCheckAndMigrate.Helpers
{
    internal class SafeLogonHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeLogonHandle()
            : base(true)
        {
        }

        public SafeLogonHandle(IntPtr handle)
            : base(true)
        {
            base.SetHandle(handle);
        }

        public IntPtr Handle
        {
            get
            {
                return this.DangerousGetHandle();
            }
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr userToken);
        }
    }
}
