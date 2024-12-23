function createInfoTvVoteBar(element, show, week, year){
			const aanestysbox = element;
			const currentDate = new Date();
			const currentDayOfWeek = currentDate.getDay();
			

			if (show) {
				fetch('/api/v1/Aanestys/Tulos?weekid=' + week.toString() + '&year=' + year.toString())
					.then(response => {
						if (!response.ok) {
							throw new Error('Network response was not ok');
						}
						return response.json();
					})
					.then(data => {
						try {
										const dayNames = ['sunnuntai', 'maanantai', 'tiistai', 'keskiviikko', 'torstai', 'perjantai', 'lauantai'];
										const dayOfWeekName = dayNames[currentDayOfWeek];
										const voteData = [];
										for (let i = 1; i < 5; i++) {
											var searchStr = "level" + i.toString() + "_votes_" + dayOfWeekName;
													voteData.push(data['votes'][searchStr])
										} 

										var sumOfVotes = (voteData[0] + voteData[1] + voteData[2] + voteData[3]);

										//jos alle 5 votee älä näytä
										if(sumOfVotes <= 5){
											aanestysbox.remove();
											console.log("alle 5 votea");
											return;
										}

										//check if aanestysbox has child votebar and if it has remove it
										var childBar = aanestysbox.querySelector('.color-bar');
										if(childBar !== null)
										{
											childBar.remove();
										}

										createPercentageBar(voteData[0], voteData[1],voteData[2], voteData[3], aanestysbox);

										var voteCounter = document.getElementById("voteCounter");
										voteCounter.innerText = sumOfVotes.toString() + " ääntä"
						} catch (err) {
							aanestysbox.remove();
							throw err;
						}
					})
					.catch(error => {
						aanestysbox.remove();
						console.error(error);
					});
			} else {
				aanestysbox.remove();
			}
}