# Hvordan genereres manntall

1. Folkeavstemning konfigureres, med gitte regler. I nåværende form støtter Folkeavstemning.Manntall følgende regelsett: kommune, alder og stemmekrets innad i kommunen.
2. Folkeavstemning.Manntall starter med å hente folkeregisteret. FolkeregisterService orkestrerer uttrekket.

   1. FolkeregisterUttrekksService starter en uttrekksjobb hos Folkeregisteret med rettighetspakke offentlig uten hjemmel, med følgende parametere: 
    ```csharp
     {
     Kommunenummer = new Kommunenummer { Bostedskommunenummer = folkeavstemning.Regler.Kommune },
     FoedselsaarTilOgMed = folkeavstemning.Regler.IkkeFødtEtter.ToString(), 
     Personstatustyper = new List<Personstatustyper>() { Personstatustyper.Bosatt}
    }
    ``` 
   2. FolkeregisterBatchService henter ned personidentifikatorer for uttrekket, 1000 om gangen
   3. FolkeregisterPersonService henter ned persondokumenter for personidentifikatorene, 1000 om gangen
   4. FolkeregisterUttrekksService lagrer personene i databasen.
3. Manntallet genereres ved å kontrollere hver person i uttrekket opp mot reglene for folkeavstemningen.
   1. GenerateManntallsnummerEndpoint henter personer for en gitt folkeavstemning
   2. Hver person blir kontrollert av StemmerettKriterier opp mot regelsettet ved å sjekke om 
      - Person er bosatt i korrekt kommune
      - Person har korrekt alder
      - Person bor i korrekt stemmekrets i kommunen
   3. Dersom personen oppfyller kriteriene blir personen tildelt et manntallsnummer, tilordning av dette gir stemmerett.
4. Hver person i manntallet blir kontrollert opp mot Kontakt og reservasjonsregisteret for å hente digitalt kontaktpunkt, og eventuell reservasjon.

Manntallet kan deretter eksporteres, til f.eks. opptelling av brevstemmer og til utsending av fysisk stemmepakke.