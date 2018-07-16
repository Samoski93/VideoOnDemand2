using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Inherit the IdentityUser class in the User class to add the basic user functionality to it.
// The User class will now be able to handle users in the database and will be used when the database is created.

namespace VideoOnDemand.Data.Data.Entities
{
    public class User : IdentityUser
    {
    }
}
