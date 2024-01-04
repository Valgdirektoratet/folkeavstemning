# Sertifikater for utvikling
Disse er lokalt generert og brukes kun i forbindelse med utvikling.

## Generering av sertifikat

1. Lag private key
`openssl genrsa -out key.pem 4096`

2. Lag CSR
`openssl req -new -sha512 -key key.pem -out csr.csr`

3. Lag x509 cert
`openssl req -x509 -sha512 -days 365 -key key.pem -in csr.csr -out certificate.pem -config openssl.cnf -extensions v3_req`

4. Lag .p12
`openssl pkcs12 -export -out dev-localhost-docker.p12 -inkey key.pem -in certificate.pem`