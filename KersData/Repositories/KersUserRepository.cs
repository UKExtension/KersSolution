using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System.Text;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kers.Models.ViewModels;

namespace Kers.Models.Repositories
{
    public class KersUserRepository : EntityBaseRepository<KersUser>, IKersUserRepository
    {

        private KERScoreContext coreContext;
        public KersUserRepository(KERScoreContext context)
            : base(context)
        { 
            this.coreContext = context;
        }
        

        public async Task<List<KesrUserBriefViewModel>> Search( SearchCriteriaViewModel criteria, bool refreshCache = false ){
            var search = coreContext.KersUser.Where( u => u.RprtngProfile.enabled == true)
                                .Skip( criteria.Skip ).Take( criteria.Take )
                                .Select( u => new KesrUserBriefViewModel{
                                        Id = u.Id,
                                        Name = u.PersonalProfile.FirstName + " " + u.PersonalProfile.LastName,
                                        PlanningUnitName = u.RprtngProfile.PlanningUnit.Name,
                                        PlanningUnitId = u.RprtngProfile.PlanningUnitId,
                                        Position = u.ExtensionPosition.Title,
                                        Title = u.PersonalProfile.ProfessionalTitle,
                                        Phone = u.PersonalProfile.OfficePhone == "" ? u.RprtngProfile.PlanningUnit.Phone : u.PersonalProfile.OfficePhone,
                                        Image = u.PersonalProfile.UploadImage != null ? u.PersonalProfile.UploadImage.UploadFile.Name : null
                                    } 
                                );
            if( criteria.OrderBy == "unit"){
                search = search.OrderBy( u => u.PlanningUnitName );
            }else{
                search = search.OrderBy( u => u.Name);
            }

            var list = await search.AsNoTracking().ToListAsync();

            return list;
        }

        public KersUser findByProfileID(int ProfileId){
            var user = this.GetSingle(t=> t.classicReportingProfileId == ProfileId, t=>t.ExtensionPosition);
            if(user != null){
                return user;
            }
            return null;
        }

        public KersUser createUserFromProfile(zEmpRptProfile profile){
            var user = new KersUser();
            user.classicReportingProfileId = profile.Id;
            user.PersonalProfile = new PersonalProfile();
            this.populatePersonalProfileName(user.PersonalProfile, profile);
            this.populatePosition(user, profile);
            this.addRoles(user, profile);
            this.coreContext.Add(user);
            this.Commit();
            return user;
        }
        private void populatePersonalProfileName(PersonalProfile personal, zEmpRptProfile reporting){
            var name = reporting.personName;

            char[] delimiterChars = { ',', ' ' };
            var splitName = name.Split(delimiterChars);
            personal.FirstName = splitName[2];
            personal.LastName = splitName[0];
        }

        private void populatePosition(KersUser user, zEmpRptProfile reporting){
            var position = this.findPosition( reporting.positionID );
            if(position != null){
                user.ExtensionPosition = position;
            }else{
                position = coreContext.ExtensionPosition.FirstOrDefault();
                user.ExtensionPosition = position;
            }
        }

        private ExtensionPosition findPosition(string positionId){
            var positions = coreContext.ExtensionPosition;
            ExtensionPosition pos = null;
            var normalizedOne = positionId.ToString();
            foreach(var position in positions){
                var normalizedTwo = position.Code.ToString();
                if(normalizedOne == normalizedTwo){
                    return position;
                }
                pos = position;
            }
            return pos;
        }

        private void addRoles(KersUser user, zEmpRptProfile profile){
            List<zEmpProfileRole> roles = new List<zEmpProfileRole>();

            if((bool)profile.isCesInServiceAdmin){
                var role = this.coreContext.zEmpRptRoleType.Where(r=>r.shortTitle == "CESADM").FirstOrDefault();
                if(role != null){
                    var r = new zEmpProfileRole();
                    r.zEmpRoleType = role;
                    r.User = user;
                    roles.Add(r);
                }
            }

            if((bool)profile.isCesInServiceTrainer){
                var role = this.coreContext.zEmpRptRoleType.Where(r=>r.shortTitle == "TRNR").FirstOrDefault();
                if(role != null){
                    var r = new zEmpProfileRole();
                    r.zEmpRoleType = role;
                    r.User = user;
                    roles.Add(r);
                }
            }

            if((bool)profile.isDD){
                var role = this.coreContext.zEmpRptRoleType.Where(r=>r.shortTitle == "DD").FirstOrDefault();
                if(role != null){
                    var r = new zEmpProfileRole();
                    r.zEmpRoleType = role;
                    r.User = user;
                    roles.Add(r);
                }
            }
            user.Roles = roles;
        }

        public List<zEmpRoleType> roles(int id){
            var connections = this.coreContext
                                    .zEmpProfileRoles
                                    .Include(c=>c.zEmpRoleType)
                                    .Where( c => c.User.Id == id);
            var roles = new List<zEmpRoleType>();
            foreach(var connection in connections){
                var roleType = connection.zEmpRoleType;
                roles.Add(roleType);
            }
            return roles;
        }

        public zEmpRoleType roleForId(int id){
            return this.coreContext.zEmpRptRoleType.Where(r => r.Id == id).FirstOrDefault();
        }


    }
}