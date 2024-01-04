# Rutine for opptelling

## Avhengigheter

- 1 intern nøkkelperson
- 1 ekstern nøkkelperson
- Maskin med gpg installert og tilgang til Folkeavstemning.Backend og Stemmemottak.Backend
- Privat krypteringsnøkkel

## Rutine for opptelling
Opptelling foregår ved at følgende prosess gjennomføres:
Valgnøkkel må dekrypteres ved at en av de interne nøkkelpersonene og en av de eksterne nøkkelpersonene komme sammen og låser opp de private krypteringsnøklene for folkeavstemningene. Denne nøkkelen sendes så til Stemmemottak.Backend for å dekryptere og telle opp stemmer. Resultatet blir signert slik at det ikke kan modifiseres, og så publisert. Kryss i manntall blir tatt ut for å prøve brevstemmer.


1. Pakk ut privat krypteringsnøkkel

   ```Shell
   tar -xvf folkeavstemning_valgnøkkel.tgz
   cd valgnøkkel
   ```
2. Dekrypter med ekstern nøkkelholder

   ```Shell
   gpg --decrypt-files kryptering_songdalen_private.pem.gpg.gpg
   gpg --decrypt-files kryptering_søgne_private.pem.gpg.gpg
   ```
3. Dekrypter med intern nøkkelholder

   ```Shell
   gpg --decrypt-files kryptering_songdalen_private.pem.gpg
   gpg --decrypt-files kryptering_søgne_private.pem.gpg
   ```
4. Start opptelling med privat krypteringsnøkkel.\
   Åpne Stemmemottak.Backend url i nettleseren og start opptelling derfra.
5. Last ned resultat
6. Signer resultatet med følgende kommando
   
   ```
   gpg --output "Opptelling digital stemmer - <folkeavstemning>.csv.sig" --detach-sig "Opptelling digitale stemmer - <folkeavstemning>.csv.sig"
   ```
7. Publiser resultat og signatur
8. Last ned kryss i manntall\
   Åpne folkeavstemning.backend url i nettleseren og last ned kryss i manntall.