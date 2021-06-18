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
