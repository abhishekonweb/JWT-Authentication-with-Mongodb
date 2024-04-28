using System;
namespace Authenticator.Models
{
	public class MongoSetting
	{
		public string ConnectionString { get; set; } = string.Empty;
		public string DatabaseName { get; set; } = string.Empty;
		public string CollectionName { get; set; } = string.Empty; 

	}
}

