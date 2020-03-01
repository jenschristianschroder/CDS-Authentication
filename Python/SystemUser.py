import sys
import json
import adal
import requests
from flask import Flask
app = Flask(__name__)



@app.route('/')
def GetSystemUser():
    if config_file:
        with open(config_file, 'r') as f:
            parameters = f.read()
        config = json.loads(parameters)

    #Get access token
    authorityUrl = (config['authorityUrl'] + '/' +  config['tenant'])
    context = adal.AuthenticationContext(authorityUrl, validate_authority=config['tenant'] != 'adfs')
    resource = config['resource']
    token = context.acquire_token_with_client_credentials(resource, config['clientId'], config['clientSecret'])
    
    #Get current user
    url = config['apiUrl'] + 'WhoAmI()'
    headers = {'user-agent': 'my-app/0.0.1', 'OData-MaxVersion': '4.0', 'OData-Version': '4.0', 'Authorization': 'Bearer ' + token['accessToken']}

    r = requests.get(url, headers=headers)
    user = json.loads(r.text)

    #Get current system user
    url = config['apiUrl'] + 'systemusers(' + user['UserId'] + ')'
    headers = {'user-agent': 'my-app/0.0.1', 'OData-MaxVersion': '4.0', 'OData-Version': '4.0', 'Authorization': 'Bearer ' + token['accessToken']}

    r = requests.get(url, headers=headers)
    
    return json.loads(r.text)


if __name__ == '__main__':
    if len(sys.argv) >= 2:
        config_file = sys.argv[1]
    else: 
        raise ValueError('No configuration found')

    app.run(host='0.0.0.0', port=8080)