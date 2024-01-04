# Hvordan konfigurere systemet

## Konfigurasjon av folkeavstmninger

Folkeavstemninger konfigureres statisk, slik at alle systemer inneholder samme konfigurasjon. Det er konfigurert opp egen konfigurasjon for lokale utviklingsmiljøer og for QA miljøer. Dette gjøres ved å sette Environment ved miljøvariabelen ASPNETCORE_ENVIRONMENT til Development, eller QA. Produksjons konfigurasjon er standard hvis ikke Environment er angitt.

## Systemkonfigurasjon

Konfigurasjon av systemet gjøres via appsettings.json filer eller via miljøvariabler, som beskrevet her: [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0). For produksjon brukes appsettings.Production.json, og i test brukes appsettings.QA.json (disse er utelatt fra publisert kildekode)

## Logging

Folkeavstemninger logger til fil, som blir plukket opp av Splunk. Konfigurasjon av disse gjøres via appsettings.json eller miljøvariabler.
Systemet bruker Serilog for logging.

## Driftsmeldinger
Dersom deler av systemet opplever ustabilitet eller nedetid er vi nødt til å kunne levere denne informasjonen til velger. Dette gjøres i en informasjonsbanner i toppen. Dette er plassert i Folkeavstemning.Frontend slik at vi ved stor sannsynlighet kan få publisert en melding fortløpende til brukerne. 

Man kan konfigurere driftsmeldinger i Folkeavstemning.Frontend. Når man kjører .NET applikasjonen kan man legge inn følgende verdier i miljøvariabler:
``` 
Driftsmeldinger__MinMelding__Tittel=Min tittel
Driftsmeldinger__MinMelding__Informasjon=Min beskrivelse
Driftsmeldinger__MinMelding__Type=Error
```

Format beskrivelse:
- `Driftsmeldinger` - Må alltid komme først.
- `MinMelding` - Kan være hva som helst, navnet på en melding. Brukes for å "linke" sammen tittel, informasjon og type.
- `Tittel` - Tittel på driftsmeldingen.
- `Informasjon` - Beskrivelse (trenger ikke være satt).
- `Type` - Kan være en av fire: "Information", "Success", "Warning", "Error". Settes som standard til "Information".

