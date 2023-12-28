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
        createFullBar("Maanantai", data["ruokalista"]["Maanantai"], [data['votes']['level1_votes_maanantai'], data['votes']['level2_votes_maanantai'], data['votes']['level3_votes_maanantai'], data['votes']['level4_votes_maanantai']], barDiv)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Tiistai", data["ruokalista"]["Tiistai"], [data['votes']['level1_votes_tiistai'], data['votes']['level2_votes_tiistai'], data['votes']['level3_votes_tiistai'], data['votes']['level4_votes_tiistai']], barDiv)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Keskiviikko", data["ruokalista"]["Keskiviikko"], [data['votes']['level1_votes_keskiviikko'], data['votes']['level2_votes_keskiviikko'], data['votes']['level3_votes_keskiviikko'], data['votes']['level4_votes_keskiviikko']], barDiv)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Torstai", data["ruokalista"]["Torstai"], [data['votes']['level1_votes_torstai'], data['votes']['level2_votes_torstai'], data['votes']['level3_votes_torstai'], data['votes']['level4_votes_torstai']], barDiv)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Perjantai", data["ruokalista"]["Perjantai"], [data['votes']['level1_votes_perjantai'], data['votes']['level2_votes_perjantai'], data['votes']['level3_votes_perjantai'], data['votes']['level4_votes_perjantai']], barDiv)

        mainDiv.appendChild(barDiv);
    }
    else {
        var noVotesTitle = document.createElement('h4');
        noVotesTitle.textContent = 'Ei äänestydataa saatavila';
        mainDiv.appendChild(noVotesTitle);
    }





    body.appendChild(mainDiv);
}

function createFullBar(day, food, votes, body) {
    //ruokateksti
    var ruokaTeksti = document.createElement('p');
    ruokaTeksti.className = 'dayTitle';
    ruokaTeksti.textContent = day + ": " + food;

    //add elements to bar div 
    body.appendChild(ruokaTeksti);


    //VOTECOUNT
    

    sumOfAllVotes = 0;
    for (let i = 0; i < votes.length; i++) {
        sumOfAllVotes += votes[i]
    }

    var voteText = document.createElement('p');
    voteText.className = 'voteCountTitle';
    voteText.textContent = sumOfAllVotes + " ääntä";

    //add elements to bar div 
    body.appendChild(voteText);

    createProcentageBar(votes[0], votes[1], votes[2], votes[3], body);

   
   
}

function createProcentageBar(level1, level2, level3, level4, body) {

    //color-bar-div
    var colorbardiv = document.createElement('div');
    colorbardiv.className = 'color-bar';

    //5 different color segments
    var sumOfAllVotes = level1 + level2 + level3 + level4;


    var level1Procentage = Math.round((level1 / sumOfAllVotes) * 100);
    var level2Procentage = Math.round((level2 / sumOfAllVotes) * 100);
    var level3Procentage = Math.round((level3 / sumOfAllVotes) * 100);
    var level4Procentage = Math.round((level4 / sumOfAllVotes) * 100);


    if (sumOfAllVotes === 0) {
        //add white bar if no votes
        var whiteBar = document.createElement('div');
        whiteBar.classList.add('color-segment');
        whiteBar.classList.add('color-base');
        whiteBar.style.borderRadius = '10px 10px 10px 10px';
        whiteBar.style.width = '100%';

       
        colorbardiv.appendChild(whiteBar);
        body.appendChild(colorbardiv);

        return;
    }



    
    var listOfProcentages = [{ 'procentage': level1Procentage, level: 1 }, { 'procentage': level2Procentage, level: 2 }, { 'procentage': level3Procentage, level: 3 }, { 'procentage': level4Procentage, level: 4 }]
    listOfProcentages = listOfProcentages.reverse()
    console.log(listOfProcentages)

    //first bar
    var bar1 = document.createElement('div');
    bar1.classList.add('color-segment');
    bar1.classList.add('color-' + listOfProcentages[0].level);
    bar1.style.borderRadius = '10px 0px 0px 10px';
    bar1.style.width = listOfProcentages[0].procentage.toString() + '%';

    var bar1text = document.createElement('p');
    bar1text.classList.add('prosentage-' + listOfProcentages[0].level);
    bar1text.textContent = listOfProcentages[0].procentage + '%';
    bar1.appendChild(bar1text);
    colorbardiv.appendChild(bar1);

    //second bar
    var bar2 = document.createElement('div');
    bar2.classList.add('color-segment');
    bar2.classList.add('color-' + listOfProcentages[1].level);
    bar2.style.width = listOfProcentages[1].procentage + '%';

    var bar2text = document.createElement('p');
    bar2text.classList.add('prosentage-' + listOfProcentages[1].level);
    bar2text.textContent = listOfProcentages[1].procentage + '%';
    bar2.appendChild(bar2text);
    colorbardiv.appendChild(bar2);

    //third bar
    var bar3 = document.createElement('div');
    bar3.classList.add('color-segment');
    bar3.classList.add('color-' + listOfProcentages[2].level);
    bar3.style.width = listOfProcentages[2].procentage + '%';

   

    var bar3text = document.createElement('p');
    bar3text.classList.add('prosentage-' + listOfProcentages[2].level);
    bar3text.textContent = listOfProcentages[2].procentage + '%';
    bar3.appendChild(bar3text);
    colorbardiv.appendChild(bar3);

    //fourth bar
    var bar4 = document.createElement('div');
    bar4.classList.add('color-segment');
    bar4.classList.add('color-' + listOfProcentages[3].level);
    bar4.style.width = listOfProcentages[3].procentage + '%';

    var bar4text = document.createElement('p');
    bar4text.classList.add('prosentage-' + listOfProcentages[3].level);
    bar4text.textContent = listOfProcentages[3].procentage + '%';
    bar4.style.borderRadius = '0px 10px 10px 0px';
    bar4.appendChild(bar4text);
    colorbardiv.appendChild(bar4);


    body.appendChild(colorbardiv);

}