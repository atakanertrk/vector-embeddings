import ssl
import requests

old_send = requests.Session.send

def _new_send(self, request, **kwargs):
    kwargs['verify'] = False
    return old_send(self, request, **kwargs)

def disable_ssl_verification_globally():
    requests.Session.send = _new_send
    ssl._create_default_https_context = ssl._create_unverified_context
