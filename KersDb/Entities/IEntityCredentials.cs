using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kers.Models.Entities
{
    public interface IEntityCredentials
    {
        int? EmployeePositionId {get; set;}
        int? zEmpRoleTypeId {get; set;}
        int? isContyStaff {get;set;}
    }
}