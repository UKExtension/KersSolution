using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;

namespace Kers.Models.Repositories
{
    public class NavSectionRepository : EntityBaseRepository<NavSection>, INavSectionRepository
    {

        private KERScoreContext coreContext;
        public NavSectionRepository(KERScoreContext context)
            : base(context)
        { 
            this.coreContext = context;
        }

        public virtual IQueryable<NavSection> AllIncludingQuery(params Expression<Func<NavSection, object>>[] includeProperties)
        {
            IQueryable<NavSection> query = this.coreContext.Set<NavSection>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public NavGroup groupWithId(int id){
            return this.coreContext.NavGroup.Where(g => g.Id == id).FirstOrDefault();
        }

        public NavGroup groupWithIdWithItems(int id){
            return this.coreContext.NavGroup.Where(g => g.Id == id).Include( g=>g.items ).FirstOrDefault();
        }
        public NavItem itemWithId(int id){
            return this.coreContext.NavItem.Find(id);
        }
        public void deleteEntity(IEntityBase entity){
            this.coreContext.Remove(entity);
        }
    }
}