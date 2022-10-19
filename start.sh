#!/bin/bash

executing_dir()
{
	dirname `readlink -f "$0"`
}

exdir="$(executing_dir)"

PATH=$PATH:$DOTNET_ROOT

dllpath="$exdir/dynamic-firewall/bin/Release/net6.0/dynamic-firewall.dll"

if [ ! -e "$dllpath" ]; then
	dotnet build dynamic-firewall -c Release
fi

dotnet "$dllpath"