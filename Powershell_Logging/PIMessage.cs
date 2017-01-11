using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowershellLogWrapper
{
	public class PIMessage
	{
		public DateTime Timestamp;
		public string Message;
		public Severity severity;
		public int id;

		public PIMessage(string _timestamp, string _message, Severity _severity, int _id) {
			bool converted = DateTime.TryParse(_timestamp, out Timestamp);
			if(!converted) {
				throw new ArgumentException("Invalid DateTime Provided");
			}
			Message = _message;
			severity = _severity;
			id = _id;
		}
		public PIMessage(DateTime _timestamp, string _message, Severity _severity, int _id)
		{
			Timestamp = _timestamp;
			Message = _message;
			severity = _severity;
			id = _id;
		}
	}	
}
