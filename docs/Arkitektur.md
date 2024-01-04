# Arkitektur

Før utvikling startet ble løsningen for folkeavstemninger designet som et fullt konfigurerbart system for alle typer folkeavstemninger. Det ville åpnet for at en hvilken som helst aktør kunne gjennomføre folkeavstemninger uten at Valgdirektoratet involveres i konfigurasjon eller opptelling. Deretter ble elementer fjernet helt ned til løsningen var så kompakt som mulig uten at det gikk på bekostning av funksjonalitet for folkeavstemningen i Søgne og Songdalen. Løsningen er derfor mulig å utvide til en målarkitetkur ved et senere tidspunkt, dersom dette skulle være ønskelig, ved å implementere de fjernede elementene.


## Oppbygging av løsningen
Løsningen er delt inn i to hovedsystemer - Folkeavstemning og Stemmemottak, som videre er inndelt i fem tjenester. Denne oppdelingen muliggjør implementeringen av blind signering, der Folkeavstemning fungerer som signerer og Stemmemottak som validator.

![structurizr-1-Oversikt](../uploads/diagrams/structurizr-1-Oversikt.png)

### Folkeavstemning
Folkeavstemning er systemet som leverer informasjon om folkeavstemninger til velgeren, og håndterer innlogging og stemmerett. Denne delen håndterer personopplysninger, men ikke stemmedata.

![structurizr-1-Folkeavstemning](../uploads/diagrams/structurizr-1-Folkeavstemning.png)

#### Folkeavstemning.Manntall
Tjeneste for å gjøre uttrekk mot folkeregisteret og lage manntall.
Eksponerer API som Folkeavstemning.Backend kan bruke for å sjekke stemmerett, og har funksjon for å laste ned manntallet.
Denne tjenesten er ikke eksponert over nettet. Kun drift og avstemningsansvarlig har tilgang til denne tjenesten, ved behov.


#### Folkeavstemning.Backend
Tjeneste for å bestemme stemmerett, signere stemmepakker, og oppdatere kryss i manntallet.
Tjenesten består av følgende endepunkter:
- Folkeavstemning - leverer data om hvilke folkeavstemninger som er aktive, og informasjon og spørsmål i hver folkeavstemning.
- Eksport av kryss i manntall - Henter informasjon om hvilke manntallsnummer som har avlagt stemme. Inneholder ikke persondata - dette må sammensettes i ettertid sammen med ekport av manntall.
- Stemmerett - Angir om velger har stemmerett i en folkeavstemning. Dette sjekkes ved å kalle Folkeavstemning.Manntall med personidentifikatoren på den innloggede brukeren.
- Stemmegivning - Signerer en blendet stemmepakke om velgeren har stemmerett.
  
Tjenesten har kun en databasetabell, `stemmegivning`, som betegner et kryss i manntall. En unique index på `folkeavstemning_id` og `manntallsnummer` sørger for at en velger kun kan avlegge én stemme.

Denne tjenesten er ikke eksponert over nettet. Kun drift og avstemningsansvarlig har tilgang til denne tjenesten, ved behov.


#### Folkeavstemning.Frontend
Tjenesten leverer SPA (en Vue.js applikasjon) til velgeren. Den fungerer som en BFF som beskrevet i [OAuth 2.0 for Browser-Based Apps Section 6.1](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-browser-based-apps-15#name-backend-for-frontend-bff), og håndterer autentiserings tokens istedet for å la dette gjøres på klienten. Ved flere instanser blir dette lagt bak en proxy med sticky sessions, slik at hvert request går til samme server.
Tjenesten består av følgende: 
- SPA frontend
- Duende.BFF sørger for korrekt håndtering av tokens
- YARP proxy-er requests til Folkeavstemning.Backend med autentiseringstoken
- Driftsmeldinger til brukere i tilfelle Valgdirektoratet opplever ustabilitet eller nedetid.

Denne tjenesten er eksponert over nettet.


### Stemmemottak
Stemmemottak er systemet som mottar og lagrer stemmer, og leverer resultat. Denne delen håndterer stemmedata, men ikke personopplysninger. Løsningen hostes på et eget domene for å unngå å sende identifiserbar data som  cookies.
![structurizr-1-Stemmemottak](../uploads/diagrams/structurizr-1-Stemmemottak.png)

#### Stemmemottak.Frontend
Tar imot stemmer og videresender til Stemmemottak.Backend for å unngå å eksponere tjenesten på internett.

#### Stemmemottak.Backend
Tar imot stemmer, verifiserer gyldighet av signatur, lagrer stemmer, utfører opptelling.

### Kommunikasjon mellom tjenester
All kommunikasjon mellom tjenestene gjøres via HTTPS.