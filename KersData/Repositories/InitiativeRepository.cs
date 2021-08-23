using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using Kers.Models.ViewModels;

namespace Kers.Models.Repositories
{
    public class InitiativeRepository : EntityBaseRepository<StrategicInitiative>, IInitiativeRepository
    {

        KERScoreContext context;
        public InitiativeRepository(KERScoreContext context)
            : base(context)
        { 
            this.context = context;
        }


        public async Task<List<ProgramIndicatorSumViewModel>> IndicatorSumPerMajorProgram(int MajorProgramId ){
            var r = new List<ProgramIndicatorSumViewModel>();

            var ProgramIndicatorsPerProgram = await this.context.ProgramIndicatorValue
                                                        .Where( i => i.ProgramIndicator.MajorProgramId == MajorProgramId)
                                                        .Include( v => v.ProgramIndicator)
                                                        .ToListAsync();
            var Groupped =  ProgramIndicatorsPerProgram.GroupBy( i => i.ProgramIndicator )
                                .Select( g => new ProgramIndicatorSumViewModel {
                                    ProgramIndicator = g.Key,
                                    Sum = g.Sum( s => s.Value )
                                })
                                .ToList();
            return Groupped;
        }
    }
}