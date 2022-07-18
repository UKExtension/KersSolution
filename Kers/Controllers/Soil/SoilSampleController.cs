using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Kers.Models.Data;
using Kers.Models.ViewModels;

namespace Kers.Controllers.Soil
{

    [Route("api/[controller]")]
    public class SoilSampleController : Controller
    {
        KERScoreContext _coreContext;
        SoilDataContext _context;

        public SoilSampleController( 
                    SoilDataContext _context,
                    KERScoreContext _coreContext
            ){
                this._context = _context;
                this._coreContext = _coreContext;

        }


        [HttpGet("forms")]
        public IActionResult GetFormTypes(){
            var forms = this._context.TypeForm.OrderBy(r => r.Code).ToList();
            return new OkObjectResult(forms);
        }

        [HttpGet("attributetypes/{formTypeId}")]
        public IActionResult AttributeTypes(int formTypeId){
            var types = this._context.SampleAttributeType.Where(t => t.TypeFormId == formTypeId).OrderBy(r => r.Order).ToList();
            return new OkObjectResult(types);
        }

        [HttpGet("attributes/{attributeTypeId}")]
        public IActionResult Attributes(int attributeTypeId){
            var types = this._context.SampleAttribute.Where(t => t.SampleAttributeTypeId == attributeTypeId).OrderBy(r => r.Name).ToList();
            return new OkObjectResult(types);
        }

        [HttpGet("billingtypes")]
        public IActionResult BillingTypes(){
            var types = this._context.BillingType.OrderBy(r => r.Id).ToList();
            return new OkObjectResult(types);
        }




    }
}