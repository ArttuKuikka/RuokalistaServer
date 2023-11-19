document.addEventListener("DOMContentLoaded", function () {
    const contentBody = document.getElementById('aanestysDiv')

    fetch('/api/v1/Aanestys/Tulokset?take=10')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            data.forEach(element => CreateAanestysBox(element, contentBody));
        })
        .catch(error => {
            var errorElement = document.createElement('h2');
            errorElement.textContent = "Virhe hakiessa äänestystietoja! varmista että selaimesi tukee javascriptiä ja päivitä sivu.";
            contentBody.appendChild(errorElement);
        });
});


function CreateAanestysBox(data, body) {
    

    //maindiv
    var mainDiv = document.createElement('div');
    mainDiv.id = 'content';

    //weektext
    var weekText = document.createElement('div');
    weekText.className = 'dayTitle';
    weekText.style.fontFamily = 'Segoe UI';
    weekText.style.color = 'white';
    weekText.style.fontSize = '2em';
    weekText.style.fontWeight = 'bold';
    weekText.textContent = 'Viikko ' + data["ruokalista"]["WeekId"];
    mainDiv.appendChild(weekText);

    //yeartext
    var yearText = document.createElement('div');
    yearText.className = 'dayTitle';
    yearText.style.fontFamily = 'Segoe UI';
    yearText.style.color = 'white';
    yearText.style.fontSize = '1.2em';
    yearText.style.fontWeight = 'bold';
    yearText.textContent = data["ruokalista"]["Year"];
    mainDiv.appendChild(yearText);

    //first bar
    //bar div
    var barDiv = document.createElement('div');

    if (data['votes'] !== null) {
        //ruokateksti maanantai
        var ruokaTeksti = document.createElement('p');
        ruokaTeksti.className = 'dayTitle';
        ruokaTeksti.textContent = 'Maanantai: ' + data["ruokalista"]["Maanantai"];

        //add elements to bar div and bar div to maindiv
        barDiv.appendChild(ruokaTeksti);

        createProcentageBar(data['votes']['level1_votes_maanantai'], data['votes']['level2_votes_maanantai'], data['votes']['level3_votes_maanantai'], data['votes']['level4_votes_maanantai'], barDiv);
        barDiv.appendChild(document.createElement('br'));
        barDiv.appendChild(document.createElement('br'));
        createProcentageBar(1, 1, 1, 1, barDiv);
        barDiv.appendChild(document.createElement('br'));
        barDiv.appendChild(document.createElement('br'));
        createProcentageBar(1, 2, 3, 4, barDiv);
        barDiv.appendChild(document.createElement('br'));
        barDiv.appendChild(document.createElement('br'));
        createProcentageBar(4, 3, 2, 1, barDiv);

        barDiv.appendChild(document.createElement('br'));
        barDiv.appendChild(document.createElement('br'));
        createProcentageBar(10, 10, 10, 10, barDiv);
        
        mainDiv.appendChild(barDiv);
    }
    else {
        var noVotesTitle = document.createElement('h4');
        noVotesTitle.textContent = 'Ei äänestydataa saatavila';
        mainDiv.appendChild(noVotesTitle);
    }





    body.appendChild(mainDiv);
}

function createProcentageBar(level1, level2, level3, level4, body) {

    //color-bar-div
    var colorbardiv = document.createElement('div');
    colorbardiv.className = 'color-bar';

    //5 different color segments
    var sumOfAllVotes_maanantai = level1 + level2 + level3 + level4;
    var level1Procentage_maanantai = Math.round((level1 / sumOfAllVotes_maanantai) * 100);
    var level2Procentage_maanantai = Math.round((level2 / sumOfAllVotes_maanantai) * 100);
    var level3Procentage_maanantai = Math.round((level3 / sumOfAllVotes_maanantai) * 100);
    var level4Procentage_maanantai = Math.round((level4 / sumOfAllVotes_maanantai) * 100);





    var listOfProcentages = [{ 'procentage': level1Procentage_maanantai, level: 1 }, { 'procentage': level2Procentage_maanantai, level: 2 }, { 'procentage': level3Procentage_maanantai, level: 3 }, { 'procentage': level4Procentage_maanantai, level: 4 }]
    listOfProcentages = listOfProcentages.sort((a, b) => b.procentage - a.procentage);
    listOfProcentages = listOfProcentages.reverse();
    console.log(listOfProcentages)

    //first bar
    var bar1 = document.createElement('div');
    bar1.classList.add('color-segment');
    bar1.classList.add('color-' + listOfProcentages[0].level);
    bar1.style.borderRadius = '10px 0px 0px 10px';
    bar1.style.width = listOfProcentages[0].procentage.toString() + '%';

    var bar1text = document.createElement('p');
    bar1text.className = 'prosentage-' + listOfProcentages[0].level;
    bar1text.textContent = listOfProcentages[0].procentage + '%';
    bar1.appendChild(bar1text);
    colorbardiv.appendChild(bar1);

    //second bar
    var bar2 = document.createElement('div');
    bar2.classList.add('color-segment');
    bar2.classList.add('color-' + listOfProcentages[1].level);
    bar2.style.width = (listOfProcentages[1].procentage - listOfProcentages[0].procentage).toString() + '%';

    var bar2text = document.createElement('p');
    bar2text.className = 'prosentage-' + listOfProcentages[1].level;
    bar2text.textContent = listOfProcentages[1].procentage + '%';
    bar2.appendChild(bar2text);
    colorbardiv.appendChild(bar2);

    //third bar
    var bar3 = document.createElement('div');
    bar3.classList.add('color-segment');
    bar3.classList.add('color-' + listOfProcentages[2].level);
    bar3.style.width = (listOfProcentages[2].procentage - (listOfProcentages[1].procentage - listOfProcentages[0].procentage)).toString() + '%';

    var bar3text = document.createElement('p');
    bar3text.className = 'prosentage-' + listOfProcentages[2].level;
    bar3text.textContent = listOfProcentages[2].procentage + '%';
    bar3.appendChild(bar3text);
    colorbardiv.appendChild(bar3);

    //fourth bar
    var bar4 = document.createElement('div');
    bar4.classList.add('color-segment');
    bar4.classList.add('color-' + listOfProcentages[3].level);
    bar4.style.width = (listOfProcentages[3].procentage - (listOfProcentages[2].procentage - listOfProcentages[1].procentage - listOfProcentages[0].procentage)).toString() + '%';

    var bar4text = document.createElement('p');
    bar4text.className = 'prosentage-' + listOfProcentages[3].level;
    bar4text.textContent = listOfProcentages[3].procentage + '%';
    bar4.appendChild(bar4text);
    colorbardiv.appendChild(bar4);

    //base bar
    var bar5 = document.createElement('div');
    bar5.classList.add('color-segment');
    bar5.classList.add('color-base');
    bar5.style.borderRadius = '0px 10px 10px 0px';
    bar5.style.width = (100 - (parseInt(bar1.style.width.replace('%', '')) + parseInt(bar2.style.width.replace('%', '')) + parseInt(bar3.style.width.replace('%', '')) + parseInt(bar4.style.width.replace('%', '')))).toString() + '%';


    var bar5text = document.createElement('p');
    bar5text.className = 'prosentage-base';
    bar5text.textContent = sumOfAllVotes_maanantai + " ääntä"
    bar5.appendChild(bar5text);
    colorbardiv.appendChild(bar5);

    body.appendChild(colorbardiv);

}