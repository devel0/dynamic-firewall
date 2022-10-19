#!/bin/bash

executing_dir()
{
	dirname `readlink -f "$0"`
}

exdir="$(executing_dir)"

$DOTNET_ROOT/dotnet "$exdir/dynamic-firewall/bin/Debug/net6.0/dynamic-firewall.dll"
