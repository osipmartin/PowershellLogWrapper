using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;
using OSIsoft.AF.Time;

namespace PowershellLogWrapper
{
	public enum Severity {
		Critical,
		Error,
		Warning,
		Informational,
		Debug
	}

    public static class PSLog
    {
		/// <summary>
		/// Get PI SDK logs and output results as string
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <param name="remoteHost">Name of remote server to gather logs from</param>
		/// <param name="severity"></param>
		/// <param name="count">How many messages can be returned</param>
		/// <param name="message">Messages must contain this text, wildcard characters supports</param>
		/// <param name="program">Messages must come from a partticular program</param>
		/// <param name="id">Message ids</param>
		/// <param name="category"></param>
		/// <param name="originatingHost"></param>
		/// <param name="originatingOSUser"></param>
		/// <returns>string</returns>
		public static string GetLogs(	string startTime = "*-10m", string endTime= "*", string remoteHost = "", Severity severity = Severity.Error, int count = int.MaxValue, string message = "*", 
										string program = "*", int[] id = null, string category = "*", string originatingHost = "*", string originatingOSUser = "*") {
			string st = new AFTime(startTime).LocalTime.ToString();
			string et = new AFTime(endTime).LocalTime.ToString();

			StringBuilder sb = new StringBuilder();
			using (PowerShell PowerShellInstance = PowerShell.Create()) {
				StringBuilder scriptbuilder = new StringBuilder();

				scriptbuilder.Append(string.Format("Get-PIMessage -StartTime \"{0}\" -EndTime \"{1}\" -SeverityType {2} -Count {3} -Message \"{4}\" -Program \"{5}\" -Category \"{6}\" -OrginatingHost \"{7}\" -OrginatingOSUser \"{8}\"", st, et, severity.ToString("G"), count, message, program, category, originatingHost, originatingOSUser));
				if(remoteHost.Length > 0) {
					scriptbuilder.Append(" -Connection (Connect-PIDataArchive -PIDataArchiveMachineName " + remoteHost + ")");
                }

				id = id ?? new int[0];
				if(id.Length > 0) {
					string idarray = string.Join(",", id);
					scriptbuilder.Insert(0, "[int[]] $ids = " + idarray + "; ");
					scriptbuilder.Append(" -id $ids");
                }

				PowerShellInstance.AddScript(scriptbuilder.ToString());
				Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

				if(PowerShellInstance.Streams.Error.Count > 0) {
					Exception e = PowerShellInstance.Streams.Error[0].Exception;
					throw new Exception(e.Message, e);
				}
				else if(PSOutput.Count <= 0) {
					sb.Append("No messages.");
				}
				else {
					foreach (PSObject outputItem in PSOutput){
						sb.Append(string.Format("{0} {1} - +ID:{2}+ {3}\n", outputItem.Properties["Severity"].Value.ToString()[0], outputItem.Properties["Timestamp"].Value, outputItem.Properties["ID"].Value, outputItem.Properties["Message"].Value));
					}
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Get PI SDK logs and output results as a PIMessage IEnumerable
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <param name="remoteHost">Name of remote server to gather logs from</param>
		/// <param name="severity"></param>
		/// <param name="count">How many messages can be returned</param>
		/// <param name="message">Messages must contain this text, wildcard characters supports</param>
		/// <param name="program">Messages must come from a partticular program</param>
		/// <param name="id">Message ids</param>
		/// <param name="category"></param>
		/// <param name="originatingHost"></param>
		/// <param name="originatingOSUser"></param>
		/// <returns></returns>
		public static IEnumerable<PIMessage> GetLogsAsList(string startTime = "*-10m", string endTime = "*", string remoteHost = "", Severity severity = Severity.Error, int count = int.MaxValue, string message = "*",
										string program = "*", int[] id = null, string category = "*", string originatingHost = "*", string originatingOSUser = "*")
		{
			string st = new AFTime(startTime).LocalTime.ToString();
			string et = new AFTime(endTime).LocalTime.ToString();

			List<PIMessage> messages = new List<PIMessage>();
			using (PowerShell PowerShellInstance = PowerShell.Create())
			{
				StringBuilder scriptbuilder = new StringBuilder();

				scriptbuilder.Append(string.Format("Get-PIMessage -StartTime \"{0}\" -EndTime \"{1}\" -SeverityType {2} -Count {3} -Message \"{4}\" -Program \"{5}\" -Category \"{6}\" -OrginatingHost \"{7}\" -OrginatingOSUser \"{8}\"", st, et, severity.ToString("G"), count, message, program, category, originatingHost, originatingOSUser));
				if (remoteHost.Length > 0)
				{
					scriptbuilder.Append("-Connection (Connect-PIDataArchive -PIDataArchiveMachineName " + remoteHost + ")");
				}

				id = id ?? new int[0];
				if (id.Length > 0)
				{
					scriptbuilder.Append("-id @(" + string.Join(",", id) + ")");
				}

				PowerShellInstance.AddScript(scriptbuilder.ToString());
				Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

				if (PowerShellInstance.Streams.Error.Count > 0)
				{
					Exception e = PowerShellInstance.Streams.Error[0].Exception;
					throw new Exception(e.Message, e);
				}
				else
				{
					foreach (PSObject outputItem in PSOutput)
					{
						messages.Add(new PIMessage(outputItem.Properties["Timestamp"].Value.ToString(),  outputItem.Properties["Message"].Value.ToString(), (Severity)Enum.Parse(typeof(Severity),outputItem.Properties["Severity"].Value.ToString(), true), (int)outputItem.Properties["ID"].Value));
					}
				}
			}
			return messages;
		}
	}
}
