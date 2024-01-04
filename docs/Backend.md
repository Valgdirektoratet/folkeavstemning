# Backend

Backend applikasjonene er skrevet i C# (.NET), med ASP.NET Core.
Kjøres lagdelt der kun Folkeavstemning.Frontend og Stemmemottak.Frontend er eksponert mot internett.
Dette begrenser angrepsflaten.
Deretter Folkeavstemning.Backend, og deretter Databaser.


## Teknologivalg
#### C# (.NET)
Backend for folkeavstemning er skrevet i C# på grunn av teamets kompetanse. .NET 8 er valgt for både ytelse og muligheter for å observere ytelse på en detaljert måte.

#### ASP.NET Core
ASP.NET Core tilbyr moderne, høytytende og modulære muligheter for å bygge sikre og skalerbare webapplikasjoner og APIer.

#### Entity Framework
Entity Framework er valgt som ORM for enkelhetens skyld. Det forenkler databasemigrering og spørringer mot databasen.

#### PostgreSQL
PostgreSQL er valgt som database for sin pålitelighet, funksjonsrikdom og ytelse, samt at Valgdirektoratet har størst kompetanse på PostgreSQL.

#### Duende BFF
Duende BFF er implementert for å håndtere Backend-for-Frontend mønsteret, og bidrar til å forenkle autentisering og sikkerhet i SPA-applikasjoner ved å håndtere brukersesjoner og token-basert autentisering på serversiden.

#### YARP
YARP (Yet Another Reverse Proxy) er brukt for å rute forespørsler mellom Frontend og Backend tjenestene. Dette brukes sammen med Duende BFF for å sikre at autentiseringstoken følger med.

#### BCrypt
BCrypt brukes til hashing av passord for autentisering mellom tjenester.

#### NodaTime
For å sikre korrekt behandling av tidspunkter har vi valgt å bruke NodaTime for å sikre korrekt behandling av åpning og lukking av avstemningsperioden.

#### Serilog
Serilog er valgt som rammeverk for logging for ytelse og konfigurerbarhet.

## Databaser og strukturer

Folkeavstemning.Manntall har en tabell i sin database, personer. Denne inneholder hele personobjektet. Det er en egen ID per person for å forenkle oppdateringer. I tillegg er det en kolonne for manntallsnummer og folkeavstemning ID.
Manntallet deles ikke mellom folkeavstemninger, så for folkeavstemningene i Søgne og Songdalen vil hver person ligge dobbelt, men da kun med manntallsnummer i den avstemningen der personen har stemmerett.

Folkeavstemning.Backend har også en tabell i sin database, stemmegivning, som består av ID, en folkeavstemning ID og et manntallsnummer. Det er en unik indeks på folkeavstemning ID og manntallsnummer som sikrer at en person ikke kan stemme to ganger.

Stemmemottak.Backend har en tabell i sin database, krypterte stemmer. Denne inneholder kryptert stemmedata. Det er en unik indeks på data og signatur, som sikrer at en stemme kun kan avgis en gang.

## Endepunkt

#### Folkeavstemning.Manntall
- manntall/{folkeavstemningId}/uttrekk  
  Henter ned et uttrekk fra folkeregisteret basert på regler for folkeavstemningen
- manntall/{folkeavstemningId}/generate-manntallsnummer  
  Oppretter manntallet basert på stemmerettskriterier for folkeavstemningen
- manntall/{folkeavstemningId}/krr  
  Henter kontaktopplysninger og reservasjon for digital kontakt fra kontakt og reservasjonsregisteret
- manntall/{folkeavstemningId}/last-ned  
  Laster ned manntallet
- manntall/{folkeavstemningId}/last-ned-trykk  
  Laster ned manntallet med kun informasjon for valgmateriale som skal trykkes
- manntall/{folkeavstemningId}/delete  
  Sletter folkeregister uttrekket og manntallet. Brukes før skjæringsdato for å få et rent manntall.
- manntall/{folkeavstemningId}/person/{identifikator}  
  Brukes av Folkeavstemning.Backend for å verifisere stemmerett
- person/{identifikator}  
  Brukes av Folkeavstemning.Backend for å hente hvilke manntall en person tilhører

#### Folkeavstemning.Frontend
- api/driftsmeldinger   
  Eventuelle driftsmeldinger

#### Folkeavstemning.Backend
- api/folkeavstemning    
  Informasjon om hvilke folkeavstemninger som er konfigurert
- api/keys/{folkeavstemningId}/encryption/public  
  Kryptering- og signeringsnøkler for en gitt folkeavstemning
- api/stemmerett/stemmerett  
  Hvilke folkeavstemning en innlogget bruker har stemmerett i
- api/stemmerett/stemmemottak-url  
  URL til stemmemottaket
- api/stemmegivning/{folkeavstemningId}  
  Signerer stemmepakke, setter kryss i manntall
- kryss-i-manntall/count   
  Antall kryss i manntall - brukes for monitorering og sammenlikning med stemmegivninger
- kryss-i-manntall/export/{folkeavstemningId}  
  Brukes av avstemningsansvarlig for å eksportere kryss i manntall for behandling av brevstemmer

#### Stemmemottak.Frontend
- status  
  Returnerer 200 dersom Folkeavstemning.Backend er operativ og kan motta stemmer
- folkeavsteming/{folkeavstemningId}  
  Mottar krypterte stemmer med signatur og videresender til Stemmemottak.Backend


#### Stemmemottak.Backend
- stemmemottak/{folkeavstemningId}  
  Mottar krypterte stemmer med signatur og lagrer gyldige stemmer
- stemmegivninger  
  Antall stemmegivninger - brukes for monitorering og sammenlikning med kryss i manntall
- resultat/{folkeavstemningId}  
  Brukes av avstemningsansvarlig for å dekryptere og telle opp stemmer

## Skalering
Systemet er i stor grad stateless - bortsett fra innloggingssession på Folkeavstemning.Frontend. Dette løses ved å bruke sticky sessions for å sørge for at samme bruker alltid treffer samme server. Resten av systemet kan skaleres opp og ned uavhengig av hverandre.

## Logging
Det er lagt vekt på å logge så lite som mulig av persondata. Samtidig logges det nok til å kunne feilsøke eller ettergå dersom man oppdager utilsiktede hendelser.

## Integrasjoner
Systemet integerer med ID Porten for autentisering av brukere, med betydelig sikkerhetsnivå (nivå 3).
I tillegg integrerer Folkeavstemning.Manntall med både Maskinporten, Folkeregisteret og Kontakt og reservasjonsregisteret for å opprette manntallet.