function main(index){
    const contentBody = document.getElementById('aanestysDiv')

    fetch('/api/v1/Aanestys/Tulokset?start=' + index.toString() +'&take=10')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // Find the index of the object with isCurrentWeek set to true
            const currentIndex = data.findIndex(obj => obj.isCurrentWeek);

            // Remove all objects before the one with isCurrentWeek set to true
            if (currentIndex !== -1) {
                data.splice(0, currentIndex);
            }

            data.forEach(element => CreateAanestysBox(element, contentBody));
        })
        .catch(error => {
            var errorElement = document.createElement('h2');
            errorElement.textContent = "Virhe hakiessa äänestystietoja! varmista että selaimesi tukee javascriptiä ja päivitä sivu.";
            contentBody.appendChild(errorElement);
            throw error;
        });
}


function CreateAanestysBox(data, body) {
    

    //maindiv
    var mainDiv = document.createElement('div');
    mainDiv.id = 'content';

    //weektext
    var weekText = document.createElement('div');
    weekText.classList.add('dayTitle');

    let currentYearValue = new Date().getFullYear();
    let currentWeek = false;
    if (data["isCurrentWeek"] === true && data["ruokalista"]["Year"] === currentYearValue) {
        weekText.classList.add('highlighted');
        currentWeek = true;
    }

    weekText.style.fontFamily = 'Ruokalistafont';
    //weekText.style.color = 'white';
    weekText.style.fontSize = '2em';
    weekText.style.fontWeight = 'bold';
    weekText.textContent = 'Viikko ' + data["ruokalista"]["WeekId"];
    mainDiv.appendChild(weekText);

    //yeartext
    var yearText = document.createElement('div');
    yearText.className = 'dayTitle';
    yearText.style.fontFamily = 'Ruokalistafont';
    //yearText.style.color = 'white';
    yearText.style.fontSize = '1.2em';
    yearText.style.fontWeight = 'bold';
    yearText.textContent = data["ruokalista"]["Year"];
    mainDiv.appendChild(yearText);


    //generate isToday bools and set them based on the server data
    let [ma, ti, ke, to, pe] = [false, false, false, false, false]
    if (currentWeek) {
        if (data['currentDay'] === 1) { ma = true }
        if (data['currentDay'] === 2) { ti = true }
        if (data['currentDay'] === 3) { ke = true }
        if (data['currentDay'] === 4) { to = true }
        if (data['currentDay'] === 5) { pe = true }
    }


    //first bar
    //bar div
    var barDiv = document.createElement('div');

    if (data['votes'] !== null) {
        createFullBar("Maanantai", data["ruokalista"]["Maanantai"], [data['votes']['level1_votes_maanantai'], data['votes']['level2_votes_maanantai'], data['votes']['level3_votes_maanantai'], data['votes']['level4_votes_maanantai']], barDiv, ma)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Tiistai", data["ruokalista"]["Tiistai"], [data['votes']['level1_votes_tiistai'], data['votes']['level2_votes_tiistai'], data['votes']['level3_votes_tiistai'], data['votes']['level4_votes_tiistai']], barDiv, ti)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Keskiviikko", data["ruokalista"]["Keskiviikko"], [data['votes']['level1_votes_keskiviikko'], data['votes']['level2_votes_keskiviikko'], data['votes']['level3_votes_keskiviikko'], data['votes']['level4_votes_keskiviikko']], barDiv, ke)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Torstai", data["ruokalista"]["Torstai"], [data['votes']['level1_votes_torstai'], data['votes']['level2_votes_torstai'], data['votes']['level3_votes_torstai'], data['votes']['level4_votes_torstai']], barDiv, to)
        barDiv.appendChild(document.createElement('br'));
        createFullBar("Perjantai", data["ruokalista"]["Perjantai"], [data['votes']['level1_votes_perjantai'], data['votes']['level2_votes_perjantai'], data['votes']['level3_votes_perjantai'], data['votes']['level4_votes_perjantai']], barDiv, pe)

        mainDiv.appendChild(barDiv);
    }
    else {
        var noVotesTitle = document.createElement('h4');
        noVotesTitle.textContent = 'Ei äänestysdataa saatavila';
        mainDiv.appendChild(noVotesTitle);
    }





    body.appendChild(mainDiv);
}

function createFullBar(day, food, votes, body, isToday) {
    //ruokateksti
    var ruokaTeksti = document.createElement('p');
    ruokaTeksti.classList.add('dayTitle');

    if (isToday) {
        ruokaTeksti.classList.add('highlighted')
    }

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

    createPercentageBar(votes[0], votes[1], votes[2], votes[3], body);

   
   
}

function createPercentageBar(level1, level2, level3, level4, body) {

    //color-bar-div
    var colorbardiv = document.createElement('div');
    colorbardiv.className = 'color-bar';

    //5 different color segments
    var sumOfAllVotes = level1 + level2 + level3 + level4;


    var level1Percentage = Math.round((level1 / sumOfAllVotes) * 100);
    var level2Percentage = Math.round((level2 / sumOfAllVotes) * 100);
    var level3Percentage = Math.round((level3 / sumOfAllVotes) * 100);
    var level4Percentage = Math.round((level4 / sumOfAllVotes) * 100);


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



    
    var listOfPercentages = [{ 'procentage': level1Percentage, level: 1 }, { 'procentage': level2Percentage, level: 2 }, { 'procentage': level3Percentage, level: 3 }, { 'procentage': level4Percentage, level: 4 }]
    listOfPercentages = listOfPercentages.reverse()

    //first bar
    var bar1 = document.createElement('div');
    bar1.classList.add('color-segment');
    bar1.classList.add('color-' + listOfPercentages[0].level);
    //bar1.style.borderRadius = '10px 0px 0px 10px';
    bar1.style.width = listOfPercentages[0].procentage.toString() + '%';

    var bar1text = document.createElement('p');
    bar1text.classList.add('percentage-' + listOfPercentages[0].level);
    bar1text.textContent = listOfPercentages[0].procentage + '%';
    if(listOfPercentages[0].procentage === 0){
        bar1text.hidden = true;
    }
    bar1.appendChild(bar1text);
    colorbardiv.appendChild(bar1);
    

    //second bar
    var bar2 = document.createElement('div');
    bar2.classList.add('color-segment');
    bar2.classList.add('color-' + listOfPercentages[1].level);
    bar2.style.width = listOfPercentages[1].procentage + '%';

    var bar2text = document.createElement('p');
    bar2text.classList.add('percentage-' + listOfPercentages[1].level);
    bar2text.textContent = listOfPercentages[1].procentage + '%';
    if(listOfPercentages[1].procentage === 0){
        bar2text.hidden = true;
    }
    bar2.appendChild(bar2text);
    colorbardiv.appendChild(bar2);
     

    //third bar
    var bar3 = document.createElement('div');
    bar3.classList.add('color-segment');
    bar3.classList.add('color-' + listOfPercentages[2].level);
    bar3.style.width = listOfPercentages[2].procentage + '%';

   

    var bar3text = document.createElement('p');
    bar3text.classList.add('percentage-' + listOfPercentages[2].level);
    bar3text.textContent = listOfPercentages[2].procentage + '%';
     if(listOfPercentages[2].procentage === 0){
        bar3text.hidden = true;
    }
    bar3.appendChild(bar3text);
    colorbardiv.appendChild(bar3);
    

    //fourth bar
    var bar4 = document.createElement('div');
    bar4.classList.add('color-segment');
    bar4.classList.add('color-' + listOfPercentages[3].level);
    bar4.style.width = listOfPercentages[3].procentage + '%';

    var bar4text = document.createElement('p');
    bar4text.classList.add('percentage-' + listOfPercentages[3].level);
    bar4text.textContent = listOfPercentages[3].procentage + '%';
    //bar4.style.borderRadius = '0px 10px 10px 0px';
    if(listOfPercentages[3].procentage === 0){
        bar4text.hidden = true;
    }
    bar4.appendChild(bar4text);
    colorbardiv.appendChild(bar4);

    //set rounded corners
    colorbardiv.childNodes.forEach((currentVal, index, arr) => {
        if ((arr[index - 1] === undefined || arr[index - 1].textContent === '0%') && (arr[index - 2] === undefined || arr[index - 2].textContent === '0%') && (arr[index - 3] === undefined || arr[index - 3].textContent === '0%')) {
            arr[index].style.borderRadius = '10px 0px 0px 10px';
        }

        if ((arr[index + 1] === undefined || arr[index + 1].textContent === '0%') && (arr[index + 2] === undefined || arr[index + 2].textContent === '0%') && (arr[index + 3] === undefined || arr[index + 3].textContent === '0%')) {
            let originalLevels = arr[index].style.borderRadius.toString().split(" ");

            if (originalLevels[0] === undefined || originalLevels[0] === null || originalLevels[0] === "") {
                originalLevels[0] = '0px';
            }
            if (originalLevels[3] === undefined || originalLevels[3] === null || originalLevels[3] === "") {
                originalLevels[3] = '0px';
            }

            arr[index].style.borderRadius = `${originalLevels[0]} 10px 10px ${originalLevels[3]}`;
        }
    });
     


    body.appendChild(colorbardiv);

}


