using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using System.Collections.Generic;

namespace Kers.Services.Abstract
{
    public interface IMembershipService
    {
        KersUser ValidateUser(string username, string password);
        KersUser CreateUser(string username, string email, string password, int[] roles);
        KersUser GetUser(int userId);
        List<zEmpRoleType> GetUserRoles(KersUser user);
        KersUser RefreshKersUser(zEmpRptProfile rptProfile);
        zEmpRptProfile RefreshRptProfile(KersUser user);
    }
}