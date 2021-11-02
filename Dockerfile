FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

ENV GITLAB_TOKEN=gitlab-ci-token:wC6cFZiaKt64xxzo39nf

ENV NXA_GIT_VERSION=DVITA-phase2

ENV NEO_VM_REPO=https://${GITLAB_TOKEN}@gitlab.teamxi.cloud/nxa/mirrors/neo-vm.git
ENV NEO_VM_DIR=neo-vm

ENV NEO_CORE_REPO=https://${GITLAB_TOKEN}@gitlab.teamxi.cloud/nxa/mirrors/neo.git
ENV NEO_CORE_DIR=neo

ENV NEO_DEVPACK_REPO=https://${GITLAB_TOKEN}@gitlab.teamxi.cloud/nxa/mirrors/neo-devpack-dotnet.git
ENV NEO_DEVPACK_DIR=neo-devpack-dotnet

ENV NXA_SC_CAAS_REPO=https://${GITLAB_TOKEN}@gitlab.teamxi.cloud/nxa/neo-frontier-launchpad-2021/nxa-sc-caas.git
ENV NXA_SC_CAAS_DIR=nxa-sc-caas-dir

RUN curl --silent --location https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get update && apt-get install -y \
build-essential  \
    git \
    zip \
    nodejs

# Clone all repos
WORKDIR /
RUN git clone ${NEO_VM_REPO} ${NEO_VM_DIR}
RUN git clone ${NEO_CORE_REPO} ${NEO_CORE_DIR}
RUN git clone ${NEO_DEVPACK_REPO} ${NEO_DEVPACK_DIR}
RUN git clone ${NXA_SC_CAAS_REPO} ${NXA_SC_CAAS_DIR}

# Build VM
WORKDIR /${NEO_VM_DIR}
RUN git checkout ${NXA_GIT_VERSION}
RUN make

# Build Core
WORKDIR /${NEO_CORE_DIR}
RUN git checkout ${NXA_GIT_VERSION}
RUN make

# Build Devpack
WORKDIR /${NEO_DEVPACK_DIR}
RUN git checkout ${NXA_GIT_VERSION}
RUN make

# Build CaaS
WORKDIR /${NXA_SC_CAAS_DIR}
RUN git checkout master
RUN make

# RUN
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS run

ENV NXA_SC_CAAS_DIR=nxa-sc-caas-dir

RUN curl --silent --location https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get update && apt-get install -y \
    build-essential \
    procps\
    nodejs

WORKDIR /${NXA_SC_CAAS_DIR}

COPY --from=builder /${NXA_SC_CAAS_DIR}/src/nxa-sc-caas/dist ./
COPY --from=builder /${NXA_SC_CAAS_DIR}/src/nxa-sc-caas/CodeEditor ./CodeEditor

VOLUME /caas-data

ENTRYPOINT ["dotnet","nxa-sc-caas.dll"]
