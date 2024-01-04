# Hvordan gjennomføres en digital folkeavstemning

Den digitale avstemningen deles opp i fem faser: Nøkkelgenerering, Manntall, Kontroll av stemmerett, Stemmegivning, og Opptelling.

### 1. Nøkkelgenerering
For å holde avstemningen hemmelig og sikkert benyttes to konsepter, kryptering og Blind Signering. Til dette trenger systemet to nøkkel-par: krypteringsnøkkel og signeringsnøkkel.

Hvert nøkkelpar er delt opp i en offentlig del og en privat del. Den offentlige delen kan låse ned data, slik at ingen andre enn de som har den private delen kan se dataen. Dette brukes til å låse ned stemmene frem til avstemningen er over. For at Valgdirektoratet ikke kan bruke den private delen før avstemningen er over blir nøkkelen selv låst ned to ganger, en gang av valgdirektoratet selv, og en gang av eksterne nøkkelpersoner. Dette sikrer at ingen kan bruke nøkkelen på egenhånd.

I tillegg til å kryptere stemmer må systemet kunne verifisere at en stemme er avlagt av en velger med stemmerett. Til dette brukes nøklene den andre veien; den private delen signerer stemmen, og kan valideres av den offentlige delen.

### 2. Manntall
Ved skjæringsdatoen blir folkeregisteret for Kristiansand kommune hentet ned fra Skatteetaten via folkeregisteret.
Alle personer blir så sjekket opp mot stemmerettskriteriene, og blir en del av manntallet for den spesifikke folkeavstemningen dersom de oppfyller kriteriene.

### 3. Kontroll av stemmerett
Når en velger logger seg på folkeavstemning.valg.no med ID porten blir personidentifikatoren (fødselsnummer eller d-nummer) brukt til å sjekke om velgeren har stemmerett i en folkeavstemning. Dersom velgeren har stemmerett i en folkeavstemning kan velgeren avgi stemme i denne avstemningen.

### 4. Stemmegivning
Dersom velger ønsker å avlegge stemme må hen først ta stilling til spørsmålet i avstemningen og velge et svaralternativ. Når velger bekrefter valget skjer følgende:

1. Offentlig del av kryptering- og signeringsnøkkel hentes fra Valgdirektoratet.
2. Stemmen blir kryptert med den offentlige delen av krypteringsnøkkelen.
3. Den krypterte stemmen blir så sendt gjennom en algoritme som heter Blind Signatur, som maskerer den krypterte stemmen for Valgdirektoratet.
4. Den maskerte stemmen blir sendt til Valgdirektoratet, sammen med velgerens personlige identifikator. Hvis velgeren har gyldig stemmerett og ikke har avlagt en tidligere stemme blir den maskerte stemmen signert, og signaturen sendes tilbake til nettleseren. Det er viktig å merke seg at signaturen kun er gyldig for den maskerte stemmen.
5. Signaturen går gjennom den motsatte prossessen som den krypterte stemmen gikk gjennom, og blir nå en gyldig signatur for den krypterte stemmen.
6. Signatur og kryptert stemme blir så sendt til Valgdirektoratet gjennom en kanal uten personopplysninger. Signaturen blir validert, og stemmen blir lagret. Ingen informasjon som kan knytte stemme og velger lagres av løsningen i dette steget.

> Les mer om blind signatur på Wikipedia: [https://en.wikipedia.org/wiki/Blind_signature](https://en.wikipedia.org/wiki/Blind_signature)

### 5. Opptelling
Når avstemningen er over åpner løsningen for at stemmer kan låses opp og telles. For å kunne låse opp stemmer må den private delen av krypteringsnøkkelen selv låses opp, ved at en av de eksterne og en av de interne nøkkelholderne (se Nøkkelgenerering) låser opp krypteringsnøkkelen sammen. Den private delen av krypteringsnøkkelen kan da sendes inn til opptellingsmodulen som låser opp stemmer, og genererer avstemningsresultatet.

### 6. Brevstemmer
Etter at de digitale stemmene er prøvd blir kryss i manntall eksportert for å prøve brevstemmer. Dersom en velger har avlagt både en digital stemme og en brevstemme vil brevstemmen bli forkastet. Resultatet av opptellingen av brevstemmer og de digitale stemmene blir så slått sammen og publisert.