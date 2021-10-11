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

start-dev:
	make -C src/nxa-sc-caas $@

start-prod:
	make -C src/nxa-sc-caas $@

.PHONY: all build restore align-project clean

.SILENT: clean

