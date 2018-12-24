# dynamic-firewall

dynamic firewall using iptables and ipset

- [prerequisites](#prerequisites)
- [features](#features)
- [how it works](#how-it-works)
- [install](#install)
  * [working copy](#working-copy)
  * [config supervisor](#config-supervisor)
  * [config json file](#config-json-file)
  * [config nginx](#config-nginx)
- [how this project was built](#how-this-project-was-built)

## prerequisites

- [dotnet](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial)
- supervisor `apt-get install supervisor`
- nginx with valid https certificate

## features

- easy webapi /Values/enable/token and /Values/disable/token to enable or disable iptables rules
- configurable through json config file
- expire rules tunable to autoremove ipaddress from ipset

## how it works

- [firewall](https://github.com/devel0/linux-scripts-utils/blob/master/fw.sh) iptables can contains rules referring set of source or destination ip address in a dynamic way by using set that can change at runtime using `ipset` command without the need to remove,add or change any iptables existing rules
- a listen process on http port 5000 serve /Values/enable and /Values/disable webapi ( nginx redirect https reverse proxy required for security reason )

## install

### working copy

```
cd /root
git clone https://github.com/devel0/dynamic-firewall.git
cd dynamic-firewall/dynamic-firewall
dotnet build
```

### config supervisor

- copy [supervisor-dynfw.conf](https://github.com/devel0/dynamic-firewall/blob/2323ec34bd2b02b26570a47e98173f02c9c16b96/supervisor-dynfw.conf) file to `/etc/supervisor/conf.d`
- `service supervisor restart`
- `ps ax | grep dotnet` checks for running dynamic-firewall process or watch at `/var/log/dynfw.log.*` for messages
- duoblecheck process respawn works by killing dynamic firewall process to see if it respawn correctly

### config json file

follows and example of `/security/dynamic-firewall.json` ( note `/security` folder must 700 mode and root owned )

```json
{
  "Validtokens" : [
	  {
	    "Token": "69b56011-d0cf-4243-bfe6-21f6f729202f",
	    "IPSetName": "support_allowed",
	    "ExpireMinutes": 360
	  }
  ]
}
```

### config nginx

follows an example of `/etc/nginx/conf.d/dynfw.conf` file that redirects https traffic coming to https://fw.my.com/dynfw/... towards http://192.168.1.254:5000 ( set [AddUrls](https://github.com/devel0/dynamic-firewall/blob/e3d58ff10819c36908e7ddf773b4d9e1bded6551/dynamic-firewall/Program.cs#L32) to specify listen address )

```
server {
	listen 443 ssl;
        listen [::]:443 ssl;

        root /var/www/html;

        server_name fw.my.com;

 location ~ /dynfw/(?<ns>.*) {                
                proxy_set_header Host $host;
                proxy_pass http://192.168.1.254:5000/$ns;
                proxy_set_header X-Real-IP $remote_addr;
                proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
}
```

## how this project was built

```sh
mkdir dynamic-firewall
cd dynamic-firewall

dotnet new webapi -n dynamic-firewall
```
