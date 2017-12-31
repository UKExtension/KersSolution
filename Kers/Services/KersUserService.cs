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

    public class KersUserService
    {
        #region Variables
        private readonly KERScoreContext _coreContext;
        
        #endregion
        public KersUserService(
            KERScoreContext _coreContext
            )
        {
            this._coreContext = _coreContext;
        }


        public KersUser syncUserFromProfile(zEmpRptProfile profile){

            var user = _coreContext.
                                KersUser.
                                Where(u => u.classicReportingProfileId == profile.Id).
                                Include(u => u.Roles).
                                Include(u => u.PersonalProfile).
                                Include(u => u.RprtngProfile).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.GeneralLocation).
                                Include(u => u.ExtensionPosition).
                                Include(u=> u.Specialties).ThenInclude(s=>s.Specialty).
                                FirstOrDefault();
            if(user == null){
                user = new KersUser();
                user.Created = DateTime.Now;
                _coreContext.Add(user);
            }
            user.classicReportingProfileId = profile.Id;
            if(user.PersonalProfile == null){
                user.PersonalProfile = new PersonalProfile();
                this.populatePersonalProfileName(user.PersonalProfile, profile);
            }
            this.populatePosition(user, profile);
            this.addRoles(user, profile);
            this.populateSpecialties(user, profile);
            this.populateReportingProfile(user, profile);

            _coreContext.SaveChanges();

            return user;
        }

        public void syncSpecialties(KersUser user, zEmpRptProfile reporting){
            populateSpecialties(user, reporting);
        }

        private void populateReportingProfile(KersUser user, zEmpRptProfile reporting){
            if(user.RprtngProfile == null){
                user.RprtngProfile = new ReportingProfile();
            }
            user.RprtngProfile.LinkBlueId = reporting.linkBlueID;
            user.RprtngProfile.PersonId = reporting.personID;
            user.RprtngProfile.Name = reporting.personName;
            user.RprtngProfile.Email = reporting.emailDeliveryAddress;
            user.RprtngProfile.EmailAlias = reporting.emailUEA;
            user.RprtngProfile.enabled = reporting.enabled??false;
            this.populateGeneralLocation(user.RprtngProfile, reporting);
            this.populatePlanningUnit(user.RprtngProfile, reporting);
            this.populateInstitution(user.RprtngProfile, reporting);
            
        }

        private void populateSpecialties(KersUser user, zEmpRptProfile profile){
            if(user.Specialties == null){
                user.Specialties = new List<KersUserSpecialty>();
            }
            if(profile.prog4HYD??false){
                this.AddToSpecialty(user.Specialties, profile, user, "prog4HYD");
            }
            if(profile.progANR??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progANR");
            }
            if(profile.progFCS??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progFCS");
            }
            if(profile.progHORT??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progHORT");
            }
            if(profile.progNEP??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progNEP");
            }
            if(profile.progOther??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progOther");
            }
        }

        private void AddToSpecialty(List<KersUserSpecialty> specialties, zEmpRptProfile profile, KersUser user, string code){
            var specialty = this._coreContext.Specialty.Where(r=>r.Code == code).FirstOrDefault();
            var isPresent = specialties.Where(s=>s.Specialty == specialty).FirstOrDefault();
            if(specialty != null && isPresent == null){
                var r = new KersUserSpecialty();
                r.Specialty = specialty;
                r.KersUserId = user.Id;
                specialties.Add(r);
            }
        }

        private void populateGeneralLocation(ReportingProfile rprtng, zEmpRptProfile profile){
            var loctn = _coreContext.GeneralLocation.Where(l=>l.Code == profile.locationID).FirstOrDefault();
            rprtng.GeneralLocation = loctn;
        }

        private void populatePlanningUnit(ReportingProfile rprtng, zEmpRptProfile profile){
            var unit = _coreContext.PlanningUnit.
                        Where(p=>p.Code == profile.planningUnitID).
                        FirstOrDefault();
            rprtng.PlanningUnit = unit;
        }
        private void populateInstitution(ReportingProfile rprtng, zEmpRptProfile profile){
            var inst = _coreContext.Institution.
                        Where(p=>p.Code == profile.instID).
                        FirstOrDefault();
            rprtng.Institution = inst;
        }

        private void populatePersonalProfileName(PersonalProfile personal, zEmpRptProfile reporting){
            var name = reporting.personName;

            char[] delimiterChars = { ',', ' ' };
            var splitName = name.Split(delimiterChars);
            if(splitName.Count() > 1){
                personal.FirstName = splitName[2];
                personal.LastName = splitName[0];
            }
        }
        private void populatePosition(KersUser user, zEmpRptProfile reporting){
            var position = _coreContext.ExtensionPosition.Where(p => p.Code == reporting.positionID).FirstOrDefault();
            if(position != null){
                user.ExtensionPosition = position;
            }else{
                if(user.ExtensionPosition == null){
                    position = _coreContext.ExtensionPosition.FirstOrDefault();
                    user.ExtensionPosition = position;
                }
            }
        }

        private void addRoles(KersUser user, zEmpRptProfile profile){
            List<zEmpProfileRole> roles;
            if(user.Roles == null){
                roles = new List<zEmpProfileRole>();
            }else{
                roles = user.Roles;
            }
            if((bool)profile.isCesInServiceAdmin){
                AddToRoles(roles, profile, user, "CESADM");
            }
            if((bool)profile.isCesInServiceTrainer){
                AddToRoles(roles, profile, user, "SRVCTRNR");
            }
            if((bool)profile.isCesInServiceAdmin){
                AddToRoles(roles, profile, user, "SRVCADM");
            }
            if((bool)profile.isDD){
                AddToRoles(roles, profile, user, "DD");
            }
            user.Roles = roles;
        }

        private void AddToRoles(List<zEmpProfileRole> roles, zEmpRptProfile profile, KersUser user, string code){
            var role = this._coreContext.zEmpRptRoleType.Where(r=>r.shortTitle == code).FirstOrDefault();
            var exists = roles.Where(r => r.zEmpRoleType == role).FirstOrDefault();
            if(role != null && exists == null){
                var r = new zEmpProfileRole();
                r.zEmpRoleType = role;
                r.User = user;
                roles.Add(r);
            }
        }

    }
}