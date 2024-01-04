Denne dokumentasjonen gir en detaljert beskrivelse av Valgdirektoratets digitale løsning for folkeavstemninger. Systemet er utformet for å sikre en sikker, anonym og effektiv prosess for gjennomføring av digitale folkeavstemninger.

## Teknisk spesifikasjon
Løsningen er skrevet i .NET 8 og Vue.js og hostes på Linux maskiner i Valgdirektoratets egne servere.

Løsningen består av 2 separate systemer: Folkeavstemning og Stemmemottak.   
* `Folkeavstemning` har følgende tjenester:
  * Lage manntall
  * Gi velgeren informasjon om folkeavstemningene
  * Signere stemme hvis velger har stemmerett
  * Levere offentlige krypteringsnøkler til velger for å kryptere stemmer før innsending
* `Stemmemottak` har følgende tjenester:
  * Motta stemmer
  * Lagre gyldige stemmer
  * Dekryptere og telle opp stemmer

## Hva er inkludert i repositoriet

Kun kjørende kode er publisert. Infrastruktur, tester, og andre filer som ikke har med selve applikasjonen er ikke med. Dette er av sikkerhetshensyn til Valgdirektoratets infrastruktur som er klassifisert som Grunnleggende Nasjonal Funksjon (GNF).