function create_tulostaulu(contentBody, startTime, endTime) {
    //muista ett� teet sellasen josta voi valita aikav�lin (custom, t�m�viikko, viimeviikko, t�m� kuukausi, viime kuukausi, viimeiset kolme kuukautta, t�m� vuosi, viime vuosi, koko aika ja custom)
    //aikav�lin keskiarvo
    //toinen valinta ett� n�ytt�� vaan tietyt p�iv�t
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
            console.log('Data:', data);
            // You can do something with the data here
        })
        .catch(error => {
            console.error('Error:', error);
        });
    
}


function formatDateForURL(date) {
    const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    const formattedDate = date.toLocaleDateString('fi-FI', options);
    return formattedDate.replace(/\//g, '-');
}