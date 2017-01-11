using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowershellLogWrapper;

namespace ReadPSLog
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(PSLog.GetLogs(startTime: "*-10m", endTime: "*", severity: Severity.Informational, count: 10, remoteHost: "server.something.com", id: new int[] {7079, 7039}));

			List<PIMessage> messages = PSLog.GetLogsAsList(startTime: "*-10m", endTime: "*", severity: Severity.Informational, count: 10, remoteHost: "server.something.com", id: new int[] { 7079, 7039 }).ToList();
			Console.ReadLine();
		}
	}
}
