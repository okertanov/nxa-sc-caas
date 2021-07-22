all:
	make -C src $@

build:
	make -C src $@

start-dev:
	make -C src $@

start-prod:
	make -C src $@

clean:
	make -C src $@

distclean:
	make -C src $@

docker-build:

docker-start-dev:

docker-start-prod:
	
docker-stop:

docker-clean:

.PHONY: all build start-dev start-prod clean distclean \
		docker-build docker-start-dev docker-start-prod docker-stop docker-clean 

.SILENT: clean distclean docker-clean
