mkfile_path := $(abspath $(lastword $(MAKEFILE_LIST)))
current_dir := $(notdir $(patsubst %/,%,$(dir $(mkfile_path))))

TARGET ?= $(current_dir)
BIN ?= bin
OUTPUT ?= $(BIN)/$(TARGET)/
ARCH ?= arm
OS ?= linux
PUSH_PORT ?= 22

all: build

clean:
	rm -rf $(BIN)
	dotnet clean

build:
	mkdir -p $(OUTPUT)
	dotnet publish --self-contained -p:PublishSingleFile=true --output $(OUTPUT) --arch $(ARCH) --os $(OS)

push: build
	rsync -a -e "ssh -p ${PUSH_PORT}" $(OUTPUT) ${PUSH_USER}@${PUSH_HOST}:/opt/tools/$(current_dir)

install:
	ssh ${PUSH_USER}@${PUSH_HOST} 'sudo mkdir -p /opt/tools/$(current_dir)'
	ssh ${PUSH_USER}@${PUSH_HOST} 'sudo chown ${PUSH_USER} /opt/tools/$(current_dir)'
	make push

push-and-run: push
	ssh ${PUSH_USER}@${PUSH_HOST} -p ${PUSH_PORT} '/opt/tools/$(current_dir)/$(current_dir)'