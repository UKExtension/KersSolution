using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;

namespace Kers.Controllers.Admin
{

    [Route("api/[controller]")]
    public class InitiativeController : BaseController
    {
        KERScoreContext coreContext;
        IInitiativeRepository repo;
        public InitiativeController( 
              KERSmainContext mainContext,
              KERScoreContext coreContext,
              IKersUserRepository userRepo,
              IInitiativeRepository repo
            ):base(mainContext, coreContext, userRepo){
            this.repo = repo;
            this.coreContext = coreContext;
        }

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id){
            var section = repo.GetSingle(id);
            return new OkObjectResult(section);
        }

        [HttpGet("All/{fy?}")]
        public IActionResult All(string fy = "0"){


            var fiscalYear = this.GetFYByName(fy, FiscalYearType.ServiceLog);

            var all = this.coreContext.StrategicInitiative.
                                    Where( i => i.FiscalYear == fiscalYear).
                                    Include(i=>i.MajorPrograms).
                                    Include(i=>i.ProgramCategory).
                                    OrderBy(i=>i.order).ToList();
            all.ForEach(i=>i.MajorPrograms = i.MajorPrograms.OrderBy(p=>p.order).ToList());
            return new OkObjectResult(all);
        }

        [HttpPost("{fyId}")]
        [Authorize]
        public IActionResult AddItitiative(int fyId, [FromBody] StrategicInitiative initiative){
            FiscalYear FiscalYear;
            if(initiative != null){
                if(fyId == 0){
                    FiscalYear = this.coreContext.FiscalYear.Where(f=>f.Type == "serviceLog").FirstOrDefault();
                }else{
                    FiscalYear = this.coreContext.FiscalYear.Find(fyId);
                }
                
                initiative.FiscalYear = FiscalYear;
                
                //var category = this.coreContext.ProgramCategory.Find(initiative.ProgramCategory.Id);
                //initiative.ProgramCategoryId = initiative.ProgramCategoryId;
                this.coreContext.StrategicInitiative.Add(initiative);
                this.coreContext.SaveChanges();

                return new OkObjectResult(initiative);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateInitiative( int id, [FromBody] StrategicInitiative initiative){
            var entity = coreContext.StrategicInitiative.Find(id);
            if(entity != null && initiative != null){
                entity.Name = initiative.Name;
                entity.ShortName = initiative.ShortName;
                entity.PacCode = initiative.PacCode;
                entity.order = initiative.order;
                entity.ProgramCategory = this.coreContext.ProgramCategory.Find(initiative.ProgramCategory.Id);
                this.coreContext.SaveChanges();
                return new OkObjectResult(entity);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete( int id){
            var entity = coreContext.StrategicInitiative.Where(i=> i.Id == id).Include(i => i.MajorPrograms).FirstOrDefault();
            if(entity != null){             
                this.coreContext.Remove(entity);
                this.coreContext.SaveChanges();
                return new OkResult();
            }else{
                return new StatusCodeResult(500);
            }
        }


        [HttpPost("program/{initiativeId}")]
        [Authorize]
        public IActionResult AddProgram( int initiativeId, [FromBody] MajorProgram program){
            
            if(program != null){
                
                var initiative = this.repo.GetSingle(i=> i.Id == initiativeId, i => i.MajorPrograms );
                initiative.MajorPrograms.Add(program);
                repo.Commit();

                return new OkObjectResult(program);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("program/{id}")]
        [Authorize]
        public IActionResult UpdateProgram( int id, [FromBody] MajorProgram program){
            MajorProgram entity = coreContext.MajorProgram.Find(id);
            if(entity != null && program != null){
                entity.Name = program.Name;
                entity.ShortName = program.ShortName;
                entity.PacCode = program.PacCode;
                entity.order = program.order;
                this.coreContext.SaveChanges();
                return new OkObjectResult(entity);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("program/{id}")]
        [Authorize]
        public IActionResult DeleteProgram( int id){
            var entity = coreContext.MajorProgram.Find(id);
            if(entity != null){             
                this.coreContext.Remove(entity);
                this.coreContext.SaveChanges();
                return new OkResult();
            }else{
                return new StatusCodeResult(500);
            }
        }





        [HttpGet("import")]
        public IActionResult Import(){


            bool saveContext = true;
            //(coreContext.MajorProgram.Count() == 0);
/* 
            FiscalYear year = coreContext.FiscalYear.Find(1);

            int initiativeOrder = 1;
            int programOrder = 1;
 */
            List<StrategicInitiative> newInitiatives = new List<StrategicInitiative>();
            List<MajorProgram> programs = new List<MajorProgram>();
            List<ProgramCategory> categories = new List<ProgramCategory>();

            var fiscalYear = this.GetFYByName("2018", FiscalYearType.ServiceLog);
            var nextFiscalYear = this.GetFYByName("2019", FiscalYearType.ServiceLog);
            var initiatives = context.StrategicInitiative.AsNoTracking().Where( i => i.FiscalYear == fiscalYear)
                                    .Include( i => i.ProgramCategory)
                                    .Include( i => i.MajorPrograms).ThenInclude(m => m.ProgramIndicators);
            foreach( var initiative in initiatives){
                initiative.Id = 0;
                initiative.FiscalYear = nextFiscalYear;

                initiative.ProgramCategory = this.context.ProgramCategory.Find(initiative.ProgramCategory.Id);

                foreach( var mp in initiative.MajorPrograms){
                    mp.Id = 0;
                    mp.ProgramCategory = initiative.ProgramCategory;
                    foreach( var pi in mp.ProgramIndicators){
                        pi.Id = 0;
                    }
                }

                newInitiatives.Add( initiative );
                context.Add(initiative);
            }

/* 

            StrategicInitiative currentInitiative = null;
            
            var pacs = mainContext.zzPacs;
            
            foreach(var pac in pacs){

                ProgramCategory category = categories.
                                                Where( c => c.ShortName == pac.cesProgCategoryNameShort).
                                                FirstOrDefault();
                if(category == null){
                    category = new ProgramCategory();
                    category.Name = pac.pacTitle;
                    category.ShortName = pac.cesProgCategoryNameShort;
                    categories.Add(category);

                    coreContext.Add(category);
                }

                if(pac.rptCodeType == 1){
                    var initiative = new StrategicInitiative();

                    initiative.Name = pac.pacTitle.Substring(4, pac.pacTitle.Length - 4);
                    initiative.FiscalYear = year;
                    initiative.PacCode = pac.pacCodeID??0;
                    initiative.ProgramCategory = category;
                    initiative.order = initiativeOrder;
                    initiatives.Add(initiative);
                    coreContext.Add(initiative);
                    initiativeOrder++;
                    currentInitiative = initiative;
                }else{
                    var program = new MajorProgram();

                    program.Name = pac.pacTitle.Substring(7, pac.pacTitle.Length - 7);
                    program.StrategicInitiative = currentInitiative;
                    program.PacCode = pac.pacCodeID??0;
                    program.ProgramCategory = category;
                    program.order = programOrder;
                    programOrder++;
                    coreContext.Add(program);
                    programs.Add(program);
                }
            }
 */
            if(saveContext && context.StrategicInitiative.Where( i => i.FiscalYear == nextFiscalYear).FirstOrDefault() == null){
                coreContext.SaveChanges();
            }
            
            return new OkObjectResult(newInitiatives);
        }
        
        [HttpGet("category")]
        public IActionResult Category(){
            var cats = coreContext.ProgramCategory.OrderBy(c => c.Order);
            return new OkObjectResult(cats);
        }



    }
}