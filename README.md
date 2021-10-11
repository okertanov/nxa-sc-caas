NXA Smart Contract compiler as a service
========================================

To build and launch
-------------------

    make clean build
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
[ ] Persistence layer  
[ ] Bearer Auth  
[ ] Dockerize it  
[ ] NEO Compiler to test  
[ ] Background task worker  
[ ] WS or other RT notify transport  

Useful commands
---------------

### Docker

    docker ps
    docker logs -f <hash>
    docker exec -it <hash> /bin/bash

### Linux

    ps
    pwd
    env
    ls
    ls -l
    ll

### GIT

    git status
    git checkout -b <branch-name>



Links
-----
[https://github.com/neo-project/neo-devpack-dotnet](https://github.com/neo-project/neo-devpack-dotnet)  
