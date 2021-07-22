NXA Smart Contract compiler as a service
========================================

To build and launch
-------------------

    make clean build
    make test
    make start-dev

    make docker-clean docker-build
    make docker-test
    make docker-start-dev


To deploy
---------

    make docker-clean docker-build
    make deploy-neo-nft
    or
    make deploy-nxa-nft


TODO
----
[ ] Background task worker
[ ] Persistence layer
[ ] NEO Compiler to add & impl
[ ] Dockerize it
[ ] Bearer Auth
[ ] WS or other RT notify transport


Links
-----
