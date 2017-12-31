using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Kers.Services.Abstract;
using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Kers.Services
{

    public class ClassicUserService
    {
        #region Variables
        private readonly KERSmainContext _mainContext;
        private readonly KERScoreContext _coreContext;
        
        #endregion
        public ClassicUserService(
            KERSmainContext _mainContext,
            KERScoreContext _coreContext
            )
        {
            this._mainContext = _mainContext;
            this._coreContext = _coreContext;
        }


        public zEmpRptProfile syncProfileFromUser(KersUser kersUser){


            kersUser = _coreContext.
                            KersUser.
                            Where(u => u.Id == kersUser.Id).
                                Include(u => u.RprtngProfile).
                                        ThenInclude(r=>r.GeneralLocation).
                                    Include(u=> u.RprtngProfile).
                                        ThenInclude(r => r.Institution).
                                    Include(u=>u.RprtngProfile).
                                        ThenInclude(r=>r.PlanningUnit).
                                Include(u => u.ExtensionPosition).
                                Include(u => u.Roles).
                                    ThenInclude(r => r.zEmpRoleType).
                                Include(u=>u.Specialties).
                                    ThenInclude(s => s.Specialty).
                            FirstOrDefault();


            var profile = _mainContext.zEmpRptProfiles.
                            Where(p => p.Id == kersUser.classicReportingProfileId).
                            FirstOrDefault();

            if(profile == null){
                profile = new zEmpRptProfile();
                profile.rDT = DateTime.Now;
                _mainContext.Add(profile);
            }
            profile.enabled = kersUser.RprtngProfile.enabled;
            profile.extensionIntern = hasRole(kersUser, "INTRN");
            if(kersUser.RprtngProfile.Institution == null){
                profile.instID = "21000-1862";
            }else{
                profile.instID = kersUser.RprtngProfile.Institution.Code;
            }
            
            profile.planningUnitID = kersUser.RprtngProfile.PlanningUnit.Code;
            profile.planningUnitName = kersUser.RprtngProfile.PlanningUnit.Name;
            profile.positionID = kersUser.ExtensionPosition.Code;
            profile.progANR = hasSpecialty(kersUser, "progANR");
            profile.progHORT = hasSpecialty(kersUser, "progHORT");
            profile.progFCS = hasSpecialty(kersUser, "progFCS");
            profile.prog4HYD = hasSpecialty(kersUser, "prog4HYD");
            profile.progFACLD = hasSpecialty(kersUser, "progFACLD");
            profile.progNEP = hasSpecialty(kersUser, "progNEP");
            profile.progOther = hasSpecialty(kersUser, "progOther");
            profile.locationID = kersUser.RprtngProfile.GeneralLocation.Code;
            profile.linkBlueID = kersUser.RprtngProfile.LinkBlueId;
            profile.personID = kersUser.RprtngProfile.PersonId;
            profile.personName = kersUser.RprtngProfile.Name;
            profile.isDD = hasRole(kersUser, "DD");
            profile.isCesInServiceTrainer = hasRole(kersUser, "SRVCTRNR");
            profile.isCesInServiceAdmin = hasRole(kersUser, "SRVCADM");
            profile.emailDeliveryAddress = kersUser.RprtngProfile.Email;
            profile.emailUEA = kersUser.RprtngProfile.EmailAlias;

            

            _mainContext.SaveChanges();

            return profile;
        }

        private bool hasSpecialty(KersUser kersUser, string specialty){
            var spec = kersUser.
                                Specialties.
                                Where(s=>s.Specialty.Code == specialty).
                                FirstOrDefault();
            if(spec != null){
                return true;
            }

            return false;
        }


        private bool hasRole(KersUser kersUser, string role){
            var roleType = kersUser.
                                Roles.
                                Where(r => r.zEmpRoleType.shortTitle == role).
                                FirstOrDefault();
            if(roleType != null){
                return true;
            }
            return false;
        }
    }
}