FROM postgres:15.4-alpine

ARG CERT_DIR=/var/lib/postgresql/cert

COPY ./cert/dev-localhost-docker.crt $CERT_DIR/server.crt
COPY ./cert/dev-localhost-docker.key $CERT_DIR/server.key

RUN chmod 600 $CERT_DIR/server.key && \
    chown postgres:postgres $CERT_DIR/server.key
