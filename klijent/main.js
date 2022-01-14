import { Fakultet } from "./fakultet.js";
import { Predmet } from "./predmet.js";
import {Rok} from "./Rok.js";

var listaPredmeta=[];

fetch("https://localhost:5001/Predmet/PreuzmiPredmete")
.then(p=>{
    p.json().then(predmeti=>{
        predmeti.forEach(el=>{
            var p = new Predmet(el.id,el.naziv);
            listaPredmeta.push(p);
        });

        fetch("https://localhost:5001/Ispit/IspitniRokovi")
        .then(p=>{
            p.json().then(rokovi=>{
                rokovi.forEach(el=>{
                    var p = new Rok(el.id,el.naziv);
                    listaRokova.push(p);
                    })
                var f = new Fakultet(listaPredmeta,listaRokova);
                f.crtaj(document.body);
      })
})

    })
})

console.log(listaPredmeta);

var listaRokova=[];




console.log(listaRokova);

