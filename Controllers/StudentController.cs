using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace zadatak1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        public FakultetContext Context { get; set; }

        public StudentController(FakultetContext context)
        {
            Context=context;
        }

        [Route("Studenti")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> Preuzmi([FromQuery] int[] rokIDs) 
        {
            /* Lazy loading
            var student = Context.Studenti.FirstOrDefault();
            student.StudentPredmet.Where(p=>p.Ocena>8); */

            //eager loading
            var studenti = Context.Studenti
                .Include(p=> p.StudentPredmet)
                .ThenInclude(p=>p.IspitniRok)
                .Include(p=>p.StudentPredmet)
                .ThenInclude(p=>p.Predmet) ;

            /* var student = studenti.Where(p=>p.Indeks==13245);
            await Context.Entry(student).Collection(p=>p.StudentPredmet).LoadAsync();

            foreach(var s in student.StudentPredmet)
            {
                await Context.Entry(s).Reference(q=>q.IspitniRok).LoadAsync();
                await Context.Entry(s).Reference(q=>q.Predmet).LoadAsync();
            } */
            
            var student = await studenti.ToListAsync();

            return Ok
            (
                student.Select(p=>
                new
                {
                    Indeks=p.Indeks,
                    Ime = p.Ime,
                    Prezime = p.Prezime,
                    Predmeti = p.StudentPredmet
                    .Where(q=>rokIDs.Contains(q.IspitniRok.ID))
                    .Select(q=>
                    new
                    {
                        Predmet = q.Predmet.Naziv,
                        GodinaPredmet = q.Predmet.Godina,
                        IspitniRok = q.IspitniRok.Naziv,
                        Ocena=q.Ocena
                    })
                }).ToList()
            );
          /*   return BadRequest("Greska neka"); */

        }

        [Route("StudentiPretraga/{rokovi}/{predmetID}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> StudentiPretraga(string rokovi,int predmetID) 
        {

            try{
            var rokIDs=rokovi.Split("a")
            .Where(x=>int.TryParse(x,out _))
            .Select(int.Parse)
            .ToList();

            var studentiPoPredmetu = Context.StudentiPredmeti
                .Include(p=> p.Student)
                .Include(p=>p.IspitniRok)
                .Include(p=>p.Predmet)
                .Where(p=>p.Predmet.ID==predmetID
                && rokIDs.Contains(p.IspitniRok.ID));

            var student = await studentiPoPredmetu.ToListAsync();

            return Ok
            (
                student.Select(p=>
                new
                {
                    Index=p.Student.Indeks,
                    Ime=p.Student.Ime,
                    Prezime=p.Student.Prezime,
                    Predmet=p.Predmet.Naziv,
                    Rok=p.IspitniRok.Naziv,
                    Ocena=p.Ocena
                
                }).ToList()
            );
            }
            catch(Exception e){
                return BadRequest(e.Message);
            }

        }

        [Route("DodatiStudenta")]
        [HttpPost]
        public async Task<ActionResult> DodajStudenta([FromBody] Student student)
        {
            if(student.Indeks<10000 || student.Indeks>20000)
            {
                return BadRequest("pogresan id ");
            }
            if(string.IsNullOrWhiteSpace(student.Ime) || student.Ime.Length>50)
            {
                return BadRequest("pogresno ime");
            }
            if(string.IsNullOrWhiteSpace(student.Prezime) || student.Prezime.Length>50)
            {
                return BadRequest("pogresno ime");
            }

            try
            {
                Context.Studenti.Add(student);
                await Context.SaveChangesAsync();
                return Ok($"Student je dodat! ID je: {student.ID}"); 
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PromenitiStudenta/{indeks}/{ime}/{prezime}")]
        [HttpPut]
        public async Task<ActionResult> Promeni(int indeks,string ime,string prezime)
        {
            if(indeks<10000||indeks>20000)
            {
                return BadRequest("Pogresan indeks");
            }

            try 
            {
                var student = Context.Studenti.Where(p=>p.Indeks==indeks).FirstOrDefault();
                if(student!=null)
                {
                    student.Ime=ime;   
                    student.Prezime=prezime;

                    await Context.SaveChangesAsync();
                    return Ok($"uspesno promenjen student! ID: {student.ID}");
                }
                else
                {
                    return BadRequest("Studnet nije pronadjen");
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PromenaFromBody")]
        [HttpPut]
        public async Task<ActionResult> PromeniBody([FromBody] Student student)
        {
            if(student.ID<=0)
            {
                return BadRequest("Pogresan Id");
            }
            //.. Ostale provere

            try
            {
                /* var studentZaPromenu = await Context.Studenti.FindAsync(student.ID);
                studentZaPromenu.Indeks=student.Indeks;
                studentZaPromenu.Ime=student.Ime;
                studentZaPromenu.Prezime=student.Prezime; */
                Context.Studenti.Update(student);


                await Context.SaveChangesAsync();
                return Ok("student je uspesno izmenjen");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("IzbrisatiStudenta/{id}")]
        [HttpDelete]
        public async Task<ActionResult> Izbrisi(int id)
        {
            if(id<=0)
            {
                return BadRequest("pogresan id");
            }

            try
            {
                var student = await Context.Studenti.FindAsync(id);
                Context.Studenti.Remove(student);
                await Context.SaveChangesAsync();
                return Ok($"Uspesno izbrsan student sa Indeksom:{student.Indeks}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}