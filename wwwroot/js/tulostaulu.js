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