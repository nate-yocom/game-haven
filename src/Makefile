mkfile_path := $(abspath $(lastword $(MAKEFILE_LIST)))
current_dir := $(notdir $(patsubst %/,%,$(dir $(mkfile_path))))

TARGET ?= ghaven
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
	mkdir -p $(OUTPUT)/data
	mkdir -p $(OUTPUT)/data/fonts
	dotnet publish --self-contained --output $(OUTPUT) --arch $(ARCH) --os $(OS)
	cp -Rf ../data/images/rendered/ $(OUTPUT)/data/
	cp -f data/* $(OUTPUT)/data/
	cp -Rf ../data/fonts/* $(OUTPUT)/data/fonts/

push: build
	rsync -a -e "ssh -p ${PUSH_PORT}" $(OUTPUT) ${PUSH_USER}@${PUSH_HOST}:/opt/ghaven

install:
	ssh ${PUSH_USER}@${PUSH_HOST} 'sudo mkdir -p /opt/ghaven'
	ssh ${PUSH_USER}@${PUSH_HOST} 'sudo chown ${PUSH_USER} /opt/ghaven'
	rsync -a -e "ssh -p ${PUSH_PORT}" data/ghaven.service ${PUSH_USER}@${PUSH_HOST}:/opt/ghaven
	ssh ${PUSH_USER}@${PUSH_HOST} 'sudo ln -f /opt/ghaven/ghaven.service /etc/systemd/system/ghaven.service'
	make push
	ssh ${PUSH_USER}@${PUSH_HOST} 'sudo systemctl start ghaven'
	ssh ${PUSH_USER}@${PUSH_HOST} 'sudo systemctl enable ghaven'
	
push-and-run: push
	ssh ${PUSH_USER}@${PUSH_HOST} -p ${PUSH_PORT} 'sudo systemctl restart ghaven && sudo journalctl -u ghaven -f'	
