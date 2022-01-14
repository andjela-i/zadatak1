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
    public class IspitController : ControllerBase
    {
        public FakultetContext Context { get; set; }

        public IspitController(FakultetContext context)
        {
            Context=context;
        }

        [Route("IspitniRokovi")]
        [HttpGet]
        public async Task<ActionResult> Rokovi()
        {
            return Ok(await Context.Rokovi.Select(p=>
            new
            {
                ID=p.ID,
                Naziv = p.Naziv
            }).ToListAsync());
        }

        [Route("DodajPolozeniIspit/{indeks}/{idPredmet}/{idRok}/{ocena}")]
        [HttpPost]
        public async Task<ActionResult> DodajIspit(int indeks,int idPredmet,int idRok,int ocena)
        {
            if(indeks<10000||indeks >20000)
            {
                return BadRequest("Pogresan broj indeksa");
            }
            // isto za sve ostale parametre

            try
            {
                var student = await Context.Studenti
                .Where(p=>p.Indeks==indeks).FirstOrDefaultAsync();

                var predmet = await Context.Predmeti
                .Where(p=>p.ID==idPredmet).FirstOrDefaultAsync();

                var ispitniRok = await Context.Rokovi
                .FindAsync(idRok); //samo kad pretrazujemo po ID-u

                Spoj s = new Spoj
                {
                    Student=student,
                    Predmet=predmet,
                    IspitniRok=ispitniRok,
                    Ocena=ocena
                };

                Context.StudentiPredmeti.Add(s);
                await Context.SaveChangesAsync();

                var podaciOStudentu = await Context.StudentiPredmeti
                        .Include(p=>p.Student)
                        .Include(p=>p.Predmet)
                        .Include(p =>p.IspitniRok)
                        .Where(p=>p.Student.Indeks==indeks)
                        .Select(p=>
                        new
                        {
                            Indeks=p.Student.Indeks,
                            Ime=p.Student.Ime,
                            Prezime=p.Student.Prezime,
                            Predmet=p.Predmet.Naziv,
                            IspitniRok=p.IspitniRok.Naziv,
                            Ocena=p.Ocena
                        }
                        ).ToListAsync();
                return Ok(podaciOStudentu);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }

}
