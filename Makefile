all:
	make -C src/nxa-sc-caas $@

build:
	make -C src/nxa-sc-caas $@

start-dev:
	make -C src/nxa-sc-caas $@

start-prod:
	make -C src/nxa-sc-caas $@

clean:
	make -C src/nxa-sc-caas $@

distclean:
	make -C src/nxa-sc-caas $@

docker-build:

docker-start-dev:

docker-start-prod:
	
docker-stop:

docker-clean:

.PHONY: all build start-dev start-prod clean distclean \
		docker-build docker-start-dev docker-start-prod docker-stop docker-clean 

.SILENT: clean distclean docker-clean
