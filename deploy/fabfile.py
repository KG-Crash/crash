# coding: utf-8

from fabric.api import *
from fabric.contrib import files
import json
import jinja2
import os
import copy

env.port = 22
env.timeout = 60

CONFIGURATION = None

@task
def environment(e):
    global CONFIGURATION

    with open(f'deploy/{e}.json') as f:
        CONFIGURATION = json.load(f)
        
    env.user = CONFIGURATION['authorization']['user']
    env.key_filename = CONFIGURATION['authorization']['key']
    env.port = CONFIGURATION['authorization']['port']

    env.roledefs = {service:list(set([x['host']['public'] for x in configs])) for service, configs in CONFIGURATION['servers'].items()}
    env.hosts = list(set([host for role in env.roledefs.values() for host in role]))

@task
def build(service='game'):
    with lcd('server'):
        local(f'docker build -t cshyeon/crash:{service} .')
        local(f'docker push cshyeon/crash:{service}')

    local(f'docker image prune -f')

@task
@parallel
def stop(service='game'):
    configs = [x for x in CONFIGURATION['servers'][service] if env.host in (x['host']['private'], x['host']['public'])]
    names = [f'crash.{service}.{i}' for i, x in enumerate(configs)]

    with settings(warn_only=True):
        sudo(f"docker stop {' '.join(names)}", quiet=True)

@task
@parallel
def remove(service='game'):
    stop(service)

    configs = [x for x in CONFIGURATION['servers'][service] if env.host in (x['host']['private'], x['host']['public'])]
    names = [f'crash.{service}.{i}' for i, x in enumerate(configs)]

    with settings(warn_only=True):
        sudo(f"docker rm {' '.join(names)}", quiet=True)

@task
@parallel
def deploy(service='game'):
    global CONFIGURATION

    image_name = f'cshyeon/crash:{service}'
    sudo(f'docker pull {image_name}')

    remove(service)

    configs = [x for x in CONFIGURATION['servers'][service] if env.host in (x['host']['private'], x['host']['public'])]
    sudo('mkdir -p /etc/crash/appsettings')
    for i, config in enumerate(configs):
        config = {'own': copy.deepcopy(config)}
        config.update(CONFIGURATION)
        # files.upload_template('appsettings.txt', f'/etc/crash/appsettings/appsettings.{i}.json', context=config, use_jinja=True, template_dir='deploy/templates', use_sudo=True) # TODO : appsettings 파일과 마운트

        container_name = f'crash.{service}.{i}'
        sudo(f"docker run -it -d --restart unless-stopped --name {container_name} -p {config['own']['port']}:{config['own']['port']} {image_name} ./main {config['own']['port']}")

@task
@parallel
def cleanup():
    sudo('docker image prune -f')