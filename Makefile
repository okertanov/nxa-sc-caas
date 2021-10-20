DOCKER_COMPOSE_FILE=docker-compose.yml

all:
	make -C src/nxa-sc-caas $@

build:
	make -C src/nxa-sc-caas $@

restore:
	make -C src/nxa-sc-caas $@

align-project:
	make -C src/nxa-sc-caas $@

clean:
	make -C src/nxa-sc-caas $@

client-build:
	make -C src/nxa-sc-caas $@

start-dev:
	make -C src/nxa-sc-caas $@

start-prod:
	make -C src/nxa-sc-caas $@

docker-build:
	docker-compose -f ${DOCKER_COMPOSE_FILE} build --parallel

docker-rebuild:
	docker-compose -f ${DOCKER_COMPOSE_FILE} build --parallel --no-cache --force-rm --pull

docker-stop:
	docker-compose -f ${DOCKER_COMPOSE_FILE} down --remove-orphans

docker-clean:
	docker-compose -f ${DOCKER_COMPOSE_FILE} rm -s -f -v

.PHONY: all build restore align-project clean\
			docker-build docker-rebuild docker-stop\
			docker-clean

.SILENT: clean docker-clean

