NXA Smart Contract compiler as a service
========================================

To build and launch
-------------------

    make clean distclean
    make build
    make start-dev

    make clean distclean
    make docker-build
    make docker-start-dev
        or for PROD
    make docker-start


TODO
----
[x] Persistence layer  
[x] Bearer Auth  
[x] Dockerize it  
[x] NEO Compiler to test  
[x] Background task worker  
[ ] WS or other RT notify transport  


Links
-----
[https://andit.atlassian.net/wiki/spaces/NXA/pages/490995782/Smart+Contract+compiler+as+a+service+Server](https://andit.atlassian.net/wiki/spaces/NXA/pages/490995782/Smart+Contract+compiler+as+a+service+Server)  
[https://github.com/neo-project/neo-devpack-dotnet](https://github.com/neo-project/neo-devpack-dotnet)  
