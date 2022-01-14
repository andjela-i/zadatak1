import { Student } from "./Student.js";

export class Fakultet{

    constructor(listaPred,listaRok){
        this.listaPredmeta=listaPred;
        this.listaRokova=listaRok;
        this.kontejner=null;
    }

    crtaj(host){
        this.kontejner=document.createElement("div");
        host.appendChild(this.kontejner);
        this.kontejner.className="glavniKont";

        let kontForma = document.createElement("div");
        kontForma.className="Forma";
        this.kontejner.appendChild(kontForma);

        let kontPrikaz = document.createElement("div");
        kontPrikaz.className="Prikaz";
        this.kontejner.appendChild(kontPrikaz);

        this.crtajFormu(kontForma);
        this.crtajPrikaz(kontPrikaz);
    }

    crtajPrikaz(host){
        var tabela=document.createElement("table");
        tabela.className="tabela";
        host.appendChild(tabela);

        var tabelahead=document.createElement("thead");
        tabela.appendChild(tabelahead);

        var tr = document.createElement("tr");
        tabelahead.appendChild(tr);

        var tabelabody=document.createElement("tbody");
        tabelabody.className="tabelaPodaci";
        tabela.appendChild(tabelabody);

        let th;
        var zagl=["Index" ,"Ime","Prezime","Predmet","Ispitni rok","Ocena"];
        zagl.forEach(el=>{
            th=document.createElement("th");
            th.innerHTML=el;
            tabelahead.appendChild(th);
        })
    }

    crtajFormu(host){


        let red = this.crtajRed(host);
        let l = document.createElement("label");
        l.innerHTML="Ispiti";
        red.appendChild(l);

        let se = document.createElement("select");
        red.appendChild(se);

        let op;
        this.listaPredmeta.forEach(element => {
            op=document.createElement("option");
            op.innerHTML=element.naziv;
            op.value=element.id;
            se.appendChild(op);
        });

        red = this.crtajRed(host);
        l = document.createElement("label");
        l.innerHTML="Rokovi";
        red.appendChild(l);

        red = this.crtajRed(host);
        let cbox = document.createElement("div");
        cbox.className="cbox"
        red.appendChild(cbox);

        let cboxL = document.createElement("div");
        cboxL.className="cboxL"
        cbox.appendChild(cboxL);

        let cboxD = document.createElement("div");
        cboxD.className="cboxD"
        cbox.appendChild(cboxD);

        let cb;
        let cbDiv;
        this.listaRokova.forEach((r,index)=>{

            cbDiv=document.createElement("div");
            cb=document.createElement("input");
            cb.type="checkbox";
            cb.value=r.id;
            cbDiv.appendChild(cb);

            l = document.createElement("label");
            l.innerHTML=r.naziv;
            cbDiv.appendChild(l);
            if(index%2==0){
                cboxL.appendChild(cbDiv);
            }
            else{
                cboxD.appendChild(cbDiv);
            }

        })

        red = this.crtajRed(host);
        let btnNadji=document.createElement("button");
        btnNadji.innerHTML="nadji";
        red.appendChild(btnNadji);
        btnNadji.onclick=(ev)=>this.nadjiStudente();

        red = this.crtajRed(host);
        l = document.createElement("label");
        l.innerHTML="Indeks";
        red.appendChild(l);
        var brojIndeksa = document.createElement("input");
        brojIndeksa.type="number";
        brojIndeksa.className="brojIndeksa";
        red.appendChild(brojIndeksa);

        red = this.crtajRed(host);
        l = document.createElement("label");
        l.innerHTML="Ocena";
        red.appendChild(l);
        var poljeOcena = document.createElement("input");
        poljeOcena.type="number";
        poljeOcena.className="ocena";
        red.appendChild(poljeOcena);

        red = this.crtajRed(host);
        let btnDodaj=document.createElement("button");
        btnDodaj.innerHTML="dodaj";
        red.appendChild(btnDodaj);
        btnDodaj.onclick=(ev)=>this.dodajStudenta(brojIndeksa.value,poljeOcena.value);

    }

    crtajRed(host){
        let red=document.createElement("div");
        red.className="red";
        host.appendChild(red);
        return red;
    }


    nadjiStudente(){
        let option1=this.kontejner.querySelector("select");
        var ispitID=option1.options[option1.selectedIndex].value;
        console.log(ispitID);

        //console.log(this.kontejner.querySelector('option:checked').value);

        let rokovi =this.kontejner.querySelectorAll("input[type='checkbox']:checked");
        console.log(rokovi);
        
        if(this.rokovi===null){
            alert("izaberite rok");
            return;
        }

        let nizRokova="";
        for(let i=0;i<rokovi.length;i++){
            nizRokova=nizRokova.concat(rokovi[i].value,"a");
        }
        console.log(nizRokova);

        this.ucitajStudente(ispitID,nizRokova);

    }

    ucitajStudente(ispitId,nizRokova){
        //StudentiPretraga/{rokovi}/{predmerID}
        fetch("https://localhost:5001/Student/StudentiPretraga/"+nizRokova+"/"+ispitId,
        {
            method:"GET"
        }).then(s=>{
            if(s.ok){
                var teloTabele= this.obrisiPrethodniSadrzaj();
                s.json().then(data=>{
                    
                    data.forEach(s=>{
                        let stud = new Student(s.index,s.ime,s.prezime,s.predmet,s.rok,s.ocena);
                        stud.crtaj(teloTabele);
                    })
                    
                })
            }
        })
    }

    obrisiPrethodniSadrzaj(){
        var teloTabele=document.querySelector(".tabelaPodaci");
        var roditelj =teloTabele.parentNode;
        roditelj.removeChild(teloTabele);

        teloTabele= document.createElement("tbody");
        teloTabele.className="tabelaPodaci";
        roditelj.appendChild(teloTabele);

        return teloTabele;
    }

    dodajStudenta(brojIndeksa,ocena){
        if(brojIndeksa===null||brojIndeksa===undefined||brojIndeksa==="")
        {
        alert("unesite broj indeksa");
        return;
        }
        if(ocena==="")
        {
        alert("unesite ocenu");
        return;
        }
        else{
            let ocenaParse = parseInt(ocena);
            if(ocenaParse<5 || ocenaParse>10){
                alert("neispravna ocena");
                return;
            }
        }

        let rokovi=this.kontejner.querySelectorAll("input[type='checkbox']:checked");
        if(rokovi===null||rokovi.length!=1){
            alert("izaberite 1 rok");
            return
        }

        let option1 = this.kontejner.querySelector("select");
        var ispitID = option1.options[option1.selectedIndex].value;

        console.log("ocena "+ocena);
        console.log("oindeks "+brojIndeksa);
        console.log(rokovi[0].value);
        console.log(ispitID);

        fetch("https://localhost:5001/Ispit/DodajPolozeniIspit/"+
        brojIndeksa+"/"+ispitID+"/"+rokovi[0].value+"/"+ocena,{
            method:"POST"
        }).then(s=>{
            if(s.ok){
                var teloTabele=this.obrisiPrethodniSadrzaj();
                s.json().then(data=>
                    {
                        console.log(data);
                        data.forEach(st=>{
                            const student = new Student(st.indeks,st.ime,st.prezime,st.predmet,st.ispitniRok,st.ocena);
                            student.crtaj(teloTabele);
                        })
                    })

            }
        })

    }
}