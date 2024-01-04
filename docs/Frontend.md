# Frontend

Frontend-applikasjonen er utformet for å gi en brukervennlig, effektiv og interaktiv opplevelse til sluttbrukerne. Den tjener som det primære brukergrensesnittet for interaksjon med folkeavstemningssystemet. Applikasjonen er utviklet med moderne teknologier for å sikre vedlikeholdbarhet, robusthet, sikkerhet og brukeropplevelse.


## Teknologi
* **[Vue.js](https://vuejs.org/)**: Et progressivt JavaScript-rammeverk som brukes for å bygge dynamiske brukergrensesnitt. Dette rammeverket ble valgt for sin enkelhet, fleksibilitet og høy ytelse, noe som er avgjørende for å skape en responsiv og intuitiv brukeropplevelse. Applikasjonen er utviklet som en Single-Page Application (SPA), noe som bidrar til en sømløs og rask brukerinteraksjon.
* **[ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0)**: ASP.NET Core er et åpen kildekode, høytytende web-rammeverk for bygging av moderne applikasjoner. Rammeverket er utviklet av Microsoft og tilbyr en modulær og kryssplattform-tilnærming for webutvikling. Brukt som Backend for Frontend (BFF) og en "reverse proxy" til Folkeavstemning.Backend.

## Klientside (Vue.js)
Klientapplikasjonen for folkeavstemningen er utviklet som en statisk Single-Page Application (SPA). En SPA er en webapplikasjon eller et nettsted som interagerer med brukeren ved dynamisk å oppdatere den nåværende siden med nye data fra en backend-server, i stedet for den tradisjonelle metoden hvor nettleseren laster nye sider helt. Dette tilnærmingen sikrer raskere overganger og en brukeropplevelse som ligner mer på en innebygd applikasjon.

Applikasjonen er bygget med Vue.js. I Vue.js er applikasjoner bygget opp av komponenter hvor hver komponent inneholder sin egen HTML, CSS og JavaScript. Dette gjør koden mer gjenbrukbar og modulær, og bidrar til en mer organisert og vedlikeholdbar kodebase.

På grunn av den fysiske separasjonen mellom Folkeavstemning.Backend og Stemmemottak.Backend, utføres kryptering og maskering av stemmen på klientsiden. Prosessen er som følger:


1. Henting av Krypteringsnøkkel: Klienten gjør et kall til backend-serveren for å hente de offentlige delene av for nøklene for kryptering og signering. Deretter krypteres stemmen i en stemmepakke.

2. Statussjekk: Klienten sjekker om stemmemottaket er online og klar til å motta stemmer.

3. Maskering for Anonymitet: Stemmen blir maskert i følge algoritmen for blind signering, for å sikre anonymitet for stemmegiveren. Dette er et kritisk skritt for å opprettholde valghemmeligheten.

4. Den maskerte stemmen blir sendt til Folkeavstemning for signering

5. Den maskerte signaturen blir avmaskert i følge algoritmen for blind signering.

6. Innsending av Stemmepakke: Den blendete stemmepakken sendes til stemmemottaket, som verifiserer signaturen.

### Pakker i applikasjonen
- **[vue](https://vuejs.org)**: Rammeverk
- **[vue-router](https://router.vuejs.org/)**: Brukes for ruting inne i applikasjonen
- **[vue-i18n](https://vue-i18n.intlify.dev/)**: Brukes for lokalisering (språk)
- **[@vueuse/core](https://vueuse.org/)**: Samling av nødvendige Vue komposisjonsverktøy
- **[pinia](https://pinia.vuejs.org/)**: Brukes for håndtering av tilstand i applikasjonen
- **[vite](https://vitejs.dev/)**: Brukes for utvikling og bygging av applikasjonen
- **[jsbn](https://github.com/andyperlitch/jsbn)**: Brukes for store tall i forbindelse med maskering av stemmer
- **[biome](https://biomejs.dev/)**: Formatering og linting av kode
- **[tailwind](https://tailwindcss.com/)**: CSS rammeverk for design
- **[@headlessui/vue](https://headlessui.com/vue/)**: Helt ustilte, fullt tilgjengelige UI-komponenter, designet for å integreres med Tailwind CSS
- **[typescript](https://www.typescriptlang.org/)**: Brukes for å få typer i JavaScript
- **[openapi-typescript-codegen](https://github.com/ferdikoomen/openapi-typescript-codegen)**: Bibliotek som genererer Typescript klienter basert på OpenAPI spesifikasjonen

## Serverside (ASP.NET Core)
Folkeavstemning sin backend for frontend (BFF) er en liten applikasjon som har fire oppgaver.

1. Den innebygde serveren ([Kestrel](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-8.0)) skal tjene alle de statiske filene til Vue.js applikasjonen (vise UI til brukeren).
2. Håndtere integrasjon mot ID-porten og holde på sesjoner for brukeren slik at alle tokens blir lagret på serveren.
3. Virke som en "[reverse proxy](https://www.cloudflare.com/learning/cdn/glossary/reverse-proxy/)" som sender kall videre til backend serveren. Dette gjør at backend serveren ikke eksponeres direkte ut på nett.
4. Vise driftsmeldinger dersom det er noen uforutsette hendelser.

For håndtering av sesjoner og integrasjon mot ID-porten brukes [Duende BFF](https://docs.duendesoftware.com/identityserver/v6/bff/). Dette er et sikkerhetsrammeverk som hjelper sluttbrukere med beskyttelse for en rekke forskjellige typer angrep. 