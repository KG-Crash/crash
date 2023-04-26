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
ENVIRONMENT = None

@task
def environment(e):
    global CONFIGURATION
    global ENVIRONMENT

    ENVIRONMENT = e
    with open(f'deploy/{e}.json') as f:
        CONFIGURATION = json.load(f)

    env.user = CONFIGURATION['authorization']['user']
    env.key_filename = CONFIGURATION['authorization']['key']
    env.port = CONFIGURATION['authorization']['port']

    env.roledefs = {service:list(set([x['host']['public'] for x in configs])) for service, configs in CONFIGURATION['servers'].items()}

    if 'haproxy' in CONFIGURATION:
        if 'master' in CONFIGURATION['haproxy']:
            env.roledefs['haproxy.master'] = [CONFIGURATION['haproxy']['master']['host']['public']]

        if 'slave' in CONFIGURATION['haproxy']:
            env.roledefs['haproxy.slave'] = [x['host']['public'] for x in CONFIGURATION['haproxy']['slave']]

        if 'sentinel' in CONFIGURATION['haproxy']:
            env.roledefs['haproxy.sentinel'] = [x['host']['public'] for x in CONFIGURATION['haproxy']['sentinel']]

        if 'haproxy' in CONFIGURATION['haproxy']:
            env.roledefs['haproxy.haproxy'] = [CONFIGURATION['haproxy']['haproxy']['host']['public']]

    env.hosts = list(set([host for role in env.roledefs.values() for host in role]))

@task
def build(service='game'):
    with lcd(f'server'):
        local(f'docker build -t cshyeon/crash:{service} --build-arg SERVICE={service} .')
        local(f'docker push cshyeon/crash:{service}')

    local(f'docker image prune -f')


@task
def stop(service='game'):
    configs = [x for x in CONFIGURATION['servers'][service] if env.host in (x['host']['private'], x['host']['public'])]
    names = [f'crash.{service}.{i}' for i, x in enumerate(configs)]

    with settings(warn_only=True):
        sudo(f"docker stop {' '.join(names)}", quiet=True)

@task
def remove(service='game'):
    stop(service)

    configs = [x for x in CONFIGURATION['servers'][service] if env.host in (x['host']['private'], x['host']['public'])]
    names = [f'crash.{service}.{i}' for i, x in enumerate(configs)]

    with settings(warn_only=True):
        sudo(f"docker rm {' '.join(names)}", quiet=True)

@task
def deploy(service='game'):
    global CONFIGURATION
    global ENVIRONMENT

    image_name = f'cshyeon/crash:{service}'
    sudo(f'docker pull {image_name}')

    remove(service)

    configs = [x for x in CONFIGURATION['servers'][service] if env.host in (x['host']['private'], x['host']['public'])]
    for i, config in enumerate(configs):
        config = {'own': copy.deepcopy(config)}
        config.update(CONFIGURATION)
        
        root = f'/etc/crash/appsettings/{i}'
        sudo(f'mkdir -p {root}')
        
        envcmd = ''
        if 'environments' in config['own']:
            envcmd = [f' -e {k}={v}' for k, v in config['own']['environments'].items()]

        container_name = f'crash.{service}.{i}'
        sudo(f"docker run -it -d --restart unless-stopped --name {container_name} -p {config['own']['port']}:{config['own']['port']} {envcmd} {image_name}")

@task
@roles('haproxy.master')
def _haproxy_master():
    global CONFIGURATION

    if 'haproxy' not in CONFIGURATION or 'master' not in CONFIGURATION['haproxy']:
        return

    config = CONFIGURATION['haproxy']['master']

    root = f'/usr/local/etc/haproxy/master'
    sudo(f'mkdir -p {root}')
    sudo(f'chown redis:redis {root}')
    sudo(f'chmod 777 {root}')
    with cd(root):
        sudo('rm -f redis.conf')
        files.upload_template('redis.conf.txt', 'redis.conf', context=config, use_jinja=True, template_dir='deploy/templates', use_sudo=True)
        sudo('chown redis:redis redis.conf')

        sudo('echo >> redis-server.log')
        sudo('chown redis:redis redis-server.log')
        sudo('chmod 777 redis-server.log')

        with settings(warn_only=True):
            container_name = f"haproxy.master"
            sudo(f'docker stop {container_name}')
            sudo(f'docker rm {container_name}')
        
        sudo(f'docker run -d --restart unless-stopped --name {container_name} --net=host -v $PWD:/usr/local/etc/redis redis:6.2.3-alpine redis-server /usr/local/etc/redis/redis.conf')

@roles('haproxy.slave')
def _haproxy_slave():
    global CONFIGURATION

    if 'haproxy' not in CONFIGURATION or 'slave' not in CONFIGURATION['haproxy']:
        return

    configs = [x for x in CONFIGURATION['haproxy']['slave'] if env.host in (x['host']['public'], x['host']['private'])]
    for i, config in enumerate(configs):
        root = f'/usr/local/etc/haproxy/slave/{i}'
        sudo(f'mkdir -p {root}')
        sudo(f'chown redis:redis {root}')
        sudo(f'chmod 777 {root}')

        with cd(root):
            sudo('rm -f redis.conf')
            files.upload_template('redis.conf.txt', 'redis.conf', context=config, use_jinja=True, template_dir='deploy/templates', use_sudo=True)
            sudo('chown redis:redis redis.conf')

            sudo('echo >> redis-server.log')
            sudo('chown redis:redis redis-server.log')
            sudo('chmod 777 redis-server.log')

            with settings(warn_only=True):
                container_name = f"haproxy.slave.{i}"
                sudo(f'docker stop {container_name}')
                sudo(f'docker rm {container_name}')

            master = CONFIGURATION['haproxy']['master']
            sudo(f"docker run -d --restart unless-stopped --name {container_name} --net=host -v $PWD:/usr/local/etc/redis redis:6.2.3-alpine redis-server /usr/local/etc/redis/redis.conf --slaveof {master['host']['private']} {master['port']}")

@roles('haproxy.sentinel')
def _haproxy_sentinel():
    global CONFIGURATION

    if 'haproxy' not in CONFIGURATION or 'sentinel' not in CONFIGURATION['haproxy']:
        return

    configs = [x for x in CONFIGURATION['haproxy']['sentinel'] if env.host in (x['host']['public'], x['host']['private'])]
    for i, config in enumerate(configs):
        root = f'/usr/local/etc/haproxy/sentinel/{i}'
        sudo(f'mkdir -p {root}')
        sudo(f'chown redis:redis {root}')
        sudo(f'chmod 777 {root}')

        with cd(root):
            sudo('rm -f redis.conf')

            context = copy.deepcopy(config)
            context.update({'master': CONFIGURATION['haproxy']['master']})
            files.upload_template('sentinel.conf.txt', 'redis.conf', context=context, use_jinja=True, template_dir='deploy/templates', use_sudo=True)
            sudo('chown redis:redis redis.conf')
            sudo('chmod 777 redis.conf')

            sudo('echo >> redis-server.log')
            sudo('chown redis:redis redis-server.log')
            sudo('chmod 777 redis-server.log')

            with settings(warn_only=True):
                container_name = f"haproxy.sentinel.{i}"
                sudo(f'docker stop {container_name}')
                sudo(f'docker rm {container_name}')

            sudo(f'docker run -d --restart unless-stopped --name {container_name} -v $PWD:/usr/local/etc/redis redis:6.2.3-alpine redis-server /usr/local/etc/redis/redis.conf --sentinel')
    
@task
@roles('haproxy.haproxy')
def _haproxy_haproxy():
    global CONFIGURATION

    if 'haproxy' not in CONFIGURATION or 'haproxy' not in CONFIGURATION['haproxy']:
        return

    config = CONFIGURATION['haproxy']['haproxy']
    root = f'/usr/local/etc/haproxy/haproxy'
    sudo(f'mkdir -p {root}')
    sudo(f'chown redis:redis {root}')
    with cd(root):
        sudo('rm -f haproxy.cfg')
        files.upload_template('haproxy.cfg.txt', 'haproxy.cfg', context=CONFIGURATION, use_jinja=True, template_dir='deploy/templates', use_sudo=True)

        with settings(warn_only=True):
            container_name = f"haproxy.haproxy"
            sudo(f'docker stop {container_name}')
            sudo(f'docker rm {container_name}')
        
        sudo(f"docker run -d --restart unless-stopped --name haproxy.haproxy -v $PWD:/etc/haproxy -p {config['port']}:6379 haproxy:1.7 haproxy -f /etc/haproxy/haproxy.cfg")

@task
def haproxy():
    execute(_haproxy_master)
    execute(_haproxy_slave)
    execute(_haproxy_sentinel)
    execute(_haproxy_haproxy)

@task
def cleanup():
    sudo('docker image prune -f')