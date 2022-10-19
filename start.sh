#!/bin/bash

executing_dir()
{
	dirname `readlink -f "$0"`
}

exdir="$(executing_dir)"

DOTNET=/usr/bin/dotnet

if [ ! -e $DOTNET ]; then
	DOTNET=/opt/dotnet/dotnet
fi

dllpath="$exdir/dynamic-firewall/bin/Release/net6.0/dynamic-firewall.dll"

if [ ! -e "$dllpath" ]; then
	$DOTNET build dynamic-firewall -c Release
fi

$DOTNET "$dllpath"