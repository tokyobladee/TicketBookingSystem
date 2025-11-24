#!/bin/bash

if [ ! -f /data/keyfile/mongodb-keyfile ]; then
    openssl rand -base64 756 > /data/keyfile/mongodb-keyfile
    chmod 400 /data/keyfile/mongodb-keyfile
    chown mongodb:mongodb /data/keyfile/mongodb-keyfile
fi

exec docker-entrypoint.sh mongod --replSet rs0 --bind_ip_all --keyFile /data/keyfile/mongodb-keyfile
