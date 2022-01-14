using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace zadatak1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredmetController : ControllerBase
    {
        public FakultetContext Context { get; set; }

        public PredmetController(FakultetContext context)
        {
            Context=context;
        }

        [Route("PreuzmiPredmete")]
        [HttpGet]
        public async Task<ActionResult> Preuzmi()
        {
            return Ok(await Context.Predmeti.Select(p=>
            new
            {
                Naziv = p.Naziv,
                Id=p.ID
            }).ToListAsync()
            );
        }
    }
}
