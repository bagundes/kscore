using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static KSHelpers.PrintHelper;

namespace KSShell
{
    public static class Print
    {
		public class Printer
		{
			public enum PrinterType
			{
				Graphics,
				Text,
				Other
			}

			public string Name;

			public PrinterType Type;

			public string Alias;

			public Printer(string name)
			{
				Name = name;
			}

			public bool ValidPrinter()
			{
				List<string> list = ListOfPrinters();
				return list.Where((string t) => t == Name).Any();
			}

			public static List<string> ListOfPrinters()
			{
				List<string> list = new List<string>();
				foreach (string printer in PrinterSettings.InstalledPrinters)
				{
					list.Add(printer);
				}
				return list;
			}
		}

		public static bool Printing(Printer printer, FileInfo file)
		{
			using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
			{
				BinaryReader br = new BinaryReader(fs);
				byte[] bytes2 = new byte[fs.Length];
				bool bSuccess2 = false;
				IntPtr pUnmanagedBytes2 = new IntPtr(0);
				int nLength = Convert.ToInt32(fs.Length);
				bytes2 = br.ReadBytes(nLength);
				pUnmanagedBytes2 = Marshal.AllocCoTaskMem(nLength);
				Marshal.Copy(bytes2, 0, pUnmanagedBytes2, nLength);
				bSuccess2 = PrinterHelper.SendBytesToPrinter(file.Name, printer.Name, pUnmanagedBytes2, nLength);
				Marshal.FreeCoTaskMem(pUnmanagedBytes2);
				return bSuccess2;
			}
		}

		public static bool Printing(Printer printer, string value, string name = null)
		{
			bool bSucess2 = false;
			int dwCount = value.Length;
			IntPtr pBytes = Marshal.StringToCoTaskMemAnsi(value);
			name = (name ?? ("KLIB." + DateTime.Now.ToString("ffffff")));
			bSucess2 = PrinterHelper.SendBytesToPrinter(name, printer.Name, pBytes, dwCount);
			Marshal.FreeCoTaskMem(pBytes);
			return bSucess2;
		}
	}
}
