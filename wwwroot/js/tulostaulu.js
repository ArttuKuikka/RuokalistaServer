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

    const url = `/api/v1/Aanestys/ProsenttiTuloksetAikavalilla?start=${formattedStartDate}&end=${formattedEndDate}`;

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
                createChart(contentBody, data)
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

        lista.push({ prosentit: element.prosentit.maanantai, ruoka: element.ruokalista.Maanantai, week: element.ruokalista.WeekId, tanaan: isToday(element.ruokalista.Year, element.ruokalista.WeekId, "Monday") })
        lista.push({ prosentit: element.prosentit.tiistai, ruoka: element.ruokalista.Tiistai, week: element.ruokalista.WeekId, tanaan: isToday(element.ruokalista.Year, element.ruokalista.WeekId, "Tuesday") })
        lista.push({ prosentit: element.prosentit.keskiviikko, ruoka: element.ruokalista.Keskiviikko, week: element.ruokalista.WeekId, tanaan: isToday(element.ruokalista.Year, element.ruokalista.WeekId, "Wednesday") })
        lista.push({ prosentit: element.prosentit.torstai, ruoka: element.ruokalista.Torstai, week: element.ruokalista.WeekId, tanaan: isToday(element.ruokalista.Year, element.ruokalista.WeekId, "Thursday") })
        lista.push({ prosentit: element.prosentit.perjantai, ruoka: element.ruokalista.Perjantai, week: element.ruokalista.WeekId, tanaan: isToday(element.ruokalista.Year, element.ruokalista.WeekId, "Friday") })
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

        if (x.tanaan) {
            ruokaTeksti.classList.add('today_oranssi')
        }

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


function createChart(contentBody, data) {

    let lista = [];
    data.forEach(function (element) {

        lista.push({ prosentit: element.prosentit.maanantai, ruoka: element.ruokalista.Maanantai, week: element.ruokalista.WeekId })
        lista.push({ prosentit: element.prosentit.tiistai, ruoka: element.ruokalista.Tiistai, week: element.ruokalista.WeekId })
        lista.push({ prosentit: element.prosentit.keskiviikko, ruoka: element.ruokalista.Keskiviikko, week: element.ruokalista.WeekId })
        lista.push({ prosentit: element.prosentit.torstai, ruoka: element.ruokalista.Torstai, week: element.ruokalista.WeekId })
        lista.push({ prosentit: element.prosentit.perjantai, ruoka: element.ruokalista.Perjantai, week: element.ruokalista.WeekId })
    });

    lista = lista.filter(obj => obj.prosentit !== 0);

    var div = document.createElement('div');
    var title = document.createElement('h2');
    title.textContent = "Ruokien sijoitus ajan funktiona"

    div.appendChild(title);

    //lisää tähä
    var canvas = document.createElement('canvas');



    var ctx = canvas.getContext('2d');

    var myLineChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: lista.map(dp => dp.ruoka),
            datasets: [{
                label: 'Ruoka',
                data: lista.map(dp => dp.prosentit),
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 2,
                fill: false
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                },
                x: {
                    display: false, // hide x-axis labels
                }
            },
            plugins: {
                legend: {
                    display: false, // hide legend
                }
            }
        }
    });

    div.appendChild(canvas);
    contentBody.appendChild(div);
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

function isToday(year, weeknumber, dayOfWeek) {
    if (new Date() === getDateFromYearWeekAndDay(year, weeknumber, dayOfWeek)) {
        return true;
    }
    else {
        return false
    }
}


//chatgpt
function getDateFromYearWeekAndDay(year, weekNumber, dayOfWeek) {
    // Create a new date object
    let date = new Date();

    // Set the year and the first day of January
    date.setFullYear(year, 0, 1);

    // Adjust to the first day of the week
    date.setDate(date.getDate() - date.getDay() + 1);

    // Add the number of weeks
    date.setDate(date.getDate() + (weekNumber - 1) * 7);

    // Add the specified day of the week
    date.setDate(date.getDate() + ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'].indexOf(dayOfWeek));

    return date;
}