DOCKER_COMPOSE_FILE=docker-compose.yml
DOCKER_COMPOSE_DEV_FILE=docker-compose.dev.yml

all:
	make -C src/nxa-sc-caas $@

build:
	make -C src/nxa-sc-caas $@

test:
	make -C src/nxa-sc-caas.UnitTests $@

clean:
	make -C src/nxa-sc-caas $@
	make -C src/nxa-sc-caas.UnitTests $@

start-dev:
	make -C src/nxa-sc-caas $@

start:
	make -C src/nxa-sc-caas $@

docker-build:
	docker-compose -f ${DOCKER_COMPOSE_FILE} -f ${DOCKER_COMPOSE_DEV_FILE} build --parallel

docker-start-dev:
	docker-compose -f ${DOCKER_COMPOSE_FILE} -f ${DOCKER_COMPOSE_DEV_FILE} up -d

docker-start:
	docker-compose -f ${DOCKER_COMPOSE_FILE} up -d

docker-rebuild:
	docker-compose -f ${DOCKER_COMPOSE_FILE} -f ${DOCKER_COMPOSE_DEV_FILE} build --parallel --no-cache --force-rm --pull

docker-stop:
	docker-compose -f ${DOCKER_COMPOSE_FILE} down --remove-orphans

docker-clean:
	docker-compose -f ${DOCKER_COMPOSE_FILE} rm -s -f -v

distclean: clean docker-clean

.PHONY: all build test clean start-dev start distclean\
		docker-build docker-rebuild docker-start-dev docker-start docker-stop docker-clean

.SILENT: clean docker-clean distclean
