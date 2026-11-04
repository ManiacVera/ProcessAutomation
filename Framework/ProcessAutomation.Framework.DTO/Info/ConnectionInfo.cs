using System.Runtime.Serialization;

namespace ProcessAutomation.Framework.DTO.Info
{
    [DataContract]
	public class ConnectionInfo
	{
		[DataMember(Name = "server")]
		public string Server { get; set; }

		[DataMember(Name = "port")]
		public int Port { get; set; }

		[DataMember(Name = "database")]
		public string Database { get; set; }

		[DataMember(Name = "username")]
		public string UserName { get; set; }

		[DataMember(Name = "password")]
		public string Password { get; set; }

		[DataMember(Name = "databaseType")]
		public string DatabaseType { get; set; }
	}
}
