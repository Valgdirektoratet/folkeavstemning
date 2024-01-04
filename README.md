# Løsning for digital folkeavstemning

> Dette er en kopi av kildekoden til valgdirektoratets løsning for Folkeavstemning og består av selve koden som kjører på Valgdirektoratets servere. Alt annet er fjernet for sikkerhetshensyn. Les mer om dette her

## Avhengigheter
- dotnet 8
- node 21
- docker


Forberedelser:

- Installer sertifikatet `/docker/cert/dev-localhost-docker.p12` på gjeldende bruker med passord changeit

  Plasser sertifikat i Klarerte rotsertifiseringsinstanser

Kjør tjenester

1. Kjør `npm install` i src\frontend mappen
2. Start frontend ved å kjøre `npm run dev` i src\frontend mappen
3. Start Dependencies ved å kjøre `docker compose up -d` i repository root mappen
4. Start følgende tjenester i rekkefølge:
    1. `dotnet run --project .\src\Manntall\Folkeavstemning.Manntall\Folkeavstemning.Manntall.csproj`
    2. `dotnet run --project .\src\Folkeavstemning\Folkeavstemning.Backend\Folkeavstemning.Backend.csproj`
    3. `dotnet run --project .\src\Folkeavstemning\Folkeavstemning.Frontend\Folkeavstemning.Frontend.csproj`
    4. `dotnet run --project .\src\Stemmegivning\Folkeavstemning.Backend\Folkeavstemning.Backend.csproj`
    5. `dotnet run --project .\src\Folkeavstemning\Folkeavstemning.Backend\Folkeavstemning.Backend.csproj`
5. Gå inn på `https://localhost:54300` for manntall
6. Gå inn på `https://localhost:54100` for frontend
7. Gå inn på `https://localhost:54200` for å hente resultat
8. Gå inn på `https://localhost:54000` for å hente kryss i manntall
9. Gå inn på `https://localhost:54600` for Seq

## Wiremock
For å hente ut manntall uten å kontakte FREG brukes Wiremock. Du kan hente ut både Søgne og Songdalen.
Sett følgende verdier i dotnet user-secrets for manntall (åpne terminal i src/Manntall/Folkeavstemning.Manntall mappa), via dotnet cli (OBS, merk at du må sette `<PATH TIL REPOSITORY>` manuelt)

```
dotnet user-secrets set "Maskinporten:Url" "http://localhost:54700/token"
dotnet user-secrets set "MaskinPorten:ClientId" "test"
dotnet user-secrets set "MaskinPorten:Virksomhetssertifikat:File" "<PATH TIL REPOSITORY>/docker/cert/virksomhetssertifikat.pfx"
dotnet user-secrets set "MaskinPorten:Virksomhetssertifikat:Password" "changeit"
dotnet user-secrets set "Folkeregister:Url" "http://localhost:54700/folkeregisteret/api/"
dotnet user-secrets set "Krr:Url" "http://localhost:54700/rest/v2/personer"
```

# Dependencies

### Oppsett

Kjør:
```
docker compose up -d
```

Installer sertifikatet `/docker/cert/dev-localhost-docker.p12` på gjeldende bruker med passord changeit.
Plasser sertifikat i Klarerte rotsertifiseringsinstanser

### Keycloak

Åpne Keycloak via https://localhost:54500/admin og logg inn med admin | admin

### Postgres

Koble til med:

| Key      | Value     |
|----------|-----------|
| Host     | localhost |
| Port     | 54___     |
| User     | postgres  |
| Password | postgres  |
| Database | ________  |

# Brukere i Keycloak

| Stemmerett i Songdalen |
|------------------------|
| 01821549805            |
| 01821948661            |
| 01823248934            |
| 01833647357            |
| 01836797538            |
| 01837197631            |
| 01840649969            |
| 01849298220            |
| 01850848773            |
| 01851748357            |

| Stemmerett i Søgne |
|--------------------|
| 01810146938        |
| 01813048589        |
| 01814996588        |
| 01817199158        |
| 01818198473        |
| 01818497713        |
| 01818798640        |
| 01819999747        |
| 01823548563        |
| 01825096961        |

| Uten stemmerett |
|-----------------|
| 01810049398     |
| 01810349855     |
| 01810649727     |
| 01810949437     |
| 01812949736     |
| 01813548654     |
| 01813849239     |
| 01814299860     |
| 01814798204     |
| 01815196992     |

## Lokal port konfigurasjon

Folkeavstemning tar beslag på porter i range 54000 - 54999.
Portnummer er bygd opp på følgende måte:
54 - folkeavstemning
0-9 - Tjeneste
0-9 - 0: Api, 2: Database
0-9 - Instans

| App                      | Backend | Database |
|--------------------------|---------|----------|
| Folkeavstemning.Backend  | 54000   | 54020    |
| Folkeavstemning.Frontend | 54100   |          |
| Stemmemottak.Backend     | 54200   | 54220    |
| Stemmemottak.Frontend    | 54300   |          |
| Folkeavstemning.Manntall | 54400   | 54320    |
| IdPorten Keycloak        | 54500   |          |
| Seq                      | 54600   |          |
| Prometheus               | 54610   |          |
| Grafana                  | 54620   |          |
| Wiremock                 | 54700   |          |
