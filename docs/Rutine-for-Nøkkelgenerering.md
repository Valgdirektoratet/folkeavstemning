# Rutine for nøkkelgenerering

Folkeavstemning er avhengig av to nøkkelpar, en for kryptering og en for signerings

## Avhengigheter

- 6 Yubikey
- 3 nøkkelpersoner fra Valgdirektoratet
- 3 nøkkelpersoner fra utenfor
- Maskin med ykman og gpg installert

## 1\. Nullstill yubikey

1. Nullstill hver Yubikey med følgende kommando

   ```Shell
   ykman openpgp reset
   ```
2. Sett Admin PIN på yubikey med følgende kommando

   ```Shell
    ykman openpgp access change-pin -a 12345678 -n <NY Admin PIN KODE>
   ```
3. Sett PIN på yubikey med følgende kommando

   ```Shell
   ykman openpgp access change-pin -P 123456 -n <NY PIN KODE>
   ```
4. Generer ny pgp nøkkel på yubikeyen med følgende steg
   - `gpg --card-edit`
   - `admin`
   - `generate`
   - `n` - Ikke lag backup av encryption key
   - `y` - Erstatt ekisterende nøkler
   - `0` for expiration time
   - Legg inn følgende for brukernavn og epost: `Intern 1`, `folkeavstemning-intern-1@valg.no` (Bruk Intern 1-3, og Ekstern 4-6)
   - `o` for å bekrefte og generere nøkler
5. Eksporter public keys for Intern 1-3 og Ekstern 4-6 og lever til drift for backup
6. Yubikey merkes med ID, forsegles i egen forseglingspose, og legges i konvolutt.
7. Utlever interne yubikeys til interne nøkkelansvarlige og eksterne yubikeys til eksterne nøkkelansvarlige.

## 2\. Generer ny signeringsnøkkel

```Shell
   mkdir drift/folkeavstemning.backend
   mkdir drift/stemmemottak.backend
   cd drift/folkeavstemning.backend
```

1. Generer et tilfeldig passord for signeringsnøkkelen. GPG krypter med public key til drift

   ```Shell
   openssl rand -out signering-songdalen-password.txt -hex 32
   openssl rand -out signering-søgne-password.txt -hex 32
   
   gpg --encrypt-files --recipient <drift1>@valg.no --recipient <drift2>@valg.no signering-*-password.txt
   ```
2. Generer en RSA nøkler (S) for å signere data. Nøklene skal passordbeskyttes.

   ```Shell
   openssl genpkey -algorithm RSA -out signering-songdalen-private.pem -aes256 -pass file:signering-songdalen-password.txt -pkeyopt rsa_keygen_bits:4096
   openssl rsa -in signering-songdalen-private.pem -pubout -out signering-songdalen-public.pem -passin file:signering-songdalen-password.txt
   
   openssl genpkey -algorithm RSA -out signering-søgne-private.pem -aes256 -pass file:signering-søgne-password.txt -pkeyopt rsa_keygen_bits:4096
   openssl rsa -in signering-søgne-private.pem -pubout -out signering-søgne-public.pem -passin file:signering-søgne-password.txt
   
   gpg --encrypt-files --recipient <drift1>@valg.no --recipient <drift2>@valg.no signering-*-private.pem
   ```
3. Slett de ukrypterte passord- og privatnøkkelfilene

   ```Shell
   del signering-*-password.txt
   del signering-*-private.pem
   ```
4. Flytt offentlig signeringsnøkkel til Stemmemottak.backend

   ```Shell
   mv signering-*-public.pem ../stemmemottak.backend
   ```

## 3\. Generer ny krypteringsnøkkel

```Shell
cd ../..
mkdir valgnøkkel
cd valgnøkkel
```

1. Generer RSA nøkler (K) for å kryptere data. Nøklene skal ikke passordbeskyttes.

   ```Shell
   openssl genrsa -out kryptering-songdalen-private.pem 4096
   openssl rsa -in kryptering-songdalen-private.pem -pubout -out kryptering-songdalen-public.pem

   openssl genrsa -out kryptering-søgne-private.pem 4096
   openssl rsa -in kryptering-søgne-private.pem -pubout -out kryptering-søgne-public.pem
   ```
2. Krypter privat nøkkel for interne nøkkelholdere

   ```Shell
   gpg --encrypt-files --recipient folkeavstemning-intern-1@valg.no --recipient folkeavstemning-intern-1@valg.no --recipient folkeavstemning-intern-2@valg.no kryptering-*-private.pem
   ```
3. Krypter den gpg krypterte nøkkelen for eksterne nøkkelholdere

   ```Shell
   gpg --encrypt-files --recipient folkeavstemning-ekstern-4@valg.no --recipient folkeavstemning-ekstern-5@valg.no --recipient folkeavstemning-ekstern-6@valg.no kryptering-*-private.pem.gpg
   ```
4. Slett de ukrypterte nøklene

   ```Shell
   del kryptering-*-private.pem
   del kryptering-*-private.pem.gpg
   ```
5. Flytt offentlig krypteringsnøkkel til driftsmappene

   ```Shell
   cp kryptering-*-public.pem ../drift/stemmemottak.backend
   mv kryptering-*-public.pem ../drift/folkeavstemning.backend
   ```

## 4\. Overlevering

```Shell
cd ..
```

1. Pakk og lever filer til drift

   ```Shell
   tar -cvzf folkeavstemning_drift.tgz drift
   tar -cvzf folkeavstemning_valgnøkkel.tgz valgnøkkel
   ```
