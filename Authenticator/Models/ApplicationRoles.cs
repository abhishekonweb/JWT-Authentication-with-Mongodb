using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Authenticator.Models
{
	[CollectionName("Roles")]
	public class ApplicationRoles : MongoIdentityRole<Guid>
	{
		
	}
}

