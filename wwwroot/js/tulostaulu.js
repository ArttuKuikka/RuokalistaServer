function create_tulostaulu(contentBody, startTime, endTime) {
    //muista että teet sellasen josta voi valita aikavälin (custom, tämäviikko, viimeviikko, tämä kuukausi, viime kuukausi, viimeiset kolme kuukautta, tämä vuosi, viime vuosi, koko aika ja custom)
    //aikavälin keskiarvo
    //toinen valinta että näyttää vaan tietyt päivät
    //ruokien arvostelujen kehitys ajan funktiona
    var startDate = new Date(startTime)
    var endDate = new Date(endTime)
    console.log("reguested for " + startDate.toISOString() + " - " + endDate.toISOString())

    const formattedStartDate = formatDateForURL(startDate);
    const formattedEndDate = formatDateForURL(endDate);

    const url = `/api/v1/Aanestys/ProsenttiTuloksetTietyllaAikavalilla?start=${formattedStartDate}&end=${formattedEndDate}`;

    //get all of the votes in procentage form

    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            contentBody.innerHTML = '';
            console.log('Data:', data);
            if (data.length === 0) {
                var element = document.createElement('h1');
                element.textContent = "Ei dataa aikavälillä"
                contentBody.appendChild(element);
            }
            else {
                //success
                createFoodBars(contentBody, data)
            }
        })
        .catch(error => {
            console.error('Error:', error);
            contentBody.innerHTML = '';
            var element = document.createElement('h1');
            element.textContent = "Virhe ladatessa dataa..."
            contentBody.appendChild(element);

            var element2 = document.createElement('p');
            element2.textContent = "Tarkista selaimen konsoli lisätietoja varten";
            contentBody.appendChild(element2);
        });
    
}


function createFoodBars(contentBody, data) {

    //sort data
    let lista = [];
    data.forEach(function (element) {

        lista.push({prosentit: element.prosentit.maanantai, ruoka: element.ruokalista.Maanantai})
        lista.push({prosentit: element.prosentit.tiistai, ruoka: element.ruokalista.Tiistai})
        lista.push({prosentit: element.prosentit.keskiviikko, ruoka: element.ruokalista.Keskiviikko})
        lista.push({prosentit: element.prosentit.torstai, ruoka: element.ruokalista.Torstai})
        lista.push({prosentit: element.prosentit.perjantai, ruoka: element.ruokalista.Perjantai})
    });

    lista = lista.filter(obj => obj.prosentit !== 0);

    lista.sort((a, b) => b.prosentit - a.prosentit);

    console.log(lista);

    //generate bar for each object in list
    var contentDiv = document.createElement('div');
    contentDiv.classList.add('contentDiv');

    var div = document.createElement('div');
    div.id = 'content';

    var title = document.createElement('h2');
    title.textContent = "Ruokien sijoitus"
    div.appendChild(title);
    div.appendChild(document.createElement('br'))

    var index = 1;
    lista.forEach(function (x) {

        var ruokaTeksti = document.createElement('p');
        ruokaTeksti.classList.add('dayTitle');
        ruokaTeksti.textContent = index.toString() + ": " + x.ruoka;

        div.appendChild(ruokaTeksti);


        var colorbardiv = document.createElement('div');
        colorbardiv.className = 'color-bar';

        var bar1 = document.createElement('div');
        bar1.classList.add('color-segment');
        //bar1.classList.add('color-4');

        var color = perc2color(round((x.prosentit * 100), 1));
        bar1.style.color = color;
        bar1.style.backgroundColor = color;

        bar1.style.borderRadius = '10px 0px 0px 10px';
        bar1.style.width = (x.prosentit * 100).toString() + '%';

        var bar1text = document.createElement('p');
        bar1text.classList.add('percentage-4');
        bar1text.textContent = round((x.prosentit * 100), 2).toString() + '%';
        
        bar1.appendChild(bar1text);
        colorbardiv.appendChild(bar1);

        //empty part of bar
        var basebar = document.createElement('div');
        basebar.classList.add('color-segment');
        basebar.classList.add('color-base');
        basebar.style.borderRadius = '0px 10px 10px 0px';
        basebar.style.width = (100 - (x.prosentit * 100)).toString() + '%';

        colorbardiv.appendChild(basebar);


        div.appendChild(colorbardiv);

        div.appendChild(document.createElement('br'))
        index += 1;
    });
    contentDiv.appendChild(div);
    contentBody.appendChild(contentDiv);
}


function formatDateForURL(date) {
    const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    const formattedDate = date.toLocaleDateString('fi-FI', options);
    return formattedDate.replace(/\//g, '-');
}

const round = (n, dp) => {
    const h = +('1'.padEnd(dp + 1, '0')) // 10 or 100 or 1000 or etc
    return Math.round(n * h) / h
}


// License: MIT - https://opensource.org/licenses/MIT
// Author: Michele Locati <michele@locati.it>
// Source: https://gist.github.com/mlocati/7210513
function perc2color(perc) {
    var r, g, b = 0;
    if (perc < 50) {
        r = 255;
        g = Math.round(5.1 * perc);
    }
    else {
        g = 255;
        r = Math.round(510 - 5.10 * perc);
    }
    var h = r * 0x10000 + g * 0x100 + b * 0x1;
    return '#' + ('000000' + h.toString(16)).slice(-6);
}