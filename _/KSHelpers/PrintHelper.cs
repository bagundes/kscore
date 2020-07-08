using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace KSHelpers
{
    class PrintHelper
    {
		public class PrinterHelper
		{
			[StructLayout(LayoutKind.Sequential)]
			public class DOCINFOA
			{
				[MarshalAs(UnmanagedType.LPStr)]
				public string pDocName;

				[MarshalAs(UnmanagedType.LPStr)]
				public string pOutputFile;

				[MarshalAs(UnmanagedType.LPStr)]
				public string pDataType;
			}

			[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "OpenPrinterA", ExactSpelling = true, SetLastError = true)]
			public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

			[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
			public static extern bool ClosePrinter(IntPtr hPrinter);

			[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "StartDocPrinterA", ExactSpelling = true, SetLastError = true)]
			public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In][MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

			[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
			public static extern bool EndDocPrinter(IntPtr hPrinter);

			[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
			public static extern bool StartPagePrinter(IntPtr hPrinter);

			[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
			public static extern bool EndPagePrinter(IntPtr hPrinter);

			[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
			public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

			public static bool SendBytesToPrinter(string name, string printerName, IntPtr pBytes, int dwCount)
			{
				int dwError2 = 0;
				int dwWritten = 0;
				IntPtr hPrinter = new IntPtr(0);
				DOCINFOA di = new DOCINFOA();
				bool bSuccess = false;
				di.pDocName = name;
				di.pDataType = "RAW";
				if (OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
				{
					if (StartDocPrinter(hPrinter, 1, di))
					{
						if (StartPagePrinter(hPrinter))
						{
							bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
							EndPagePrinter(hPrinter);
						}
						EndDocPrinter(hPrinter);
					}
					ClosePrinter(hPrinter);
				}
				if (!bSuccess)
				{
					dwError2 = Marshal.GetLastWin32Error();
				}
				return bSuccess;
			}
		}
	}
}
