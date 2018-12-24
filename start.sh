#!/bin/bash

executing_dir()
{
	dirname `readlink -f "$0"`
}

exdir="$(executing_dir)"

/usr/bin/dotnet "$exdir/dynamic-firewall/bin/Debug/netcoreapp2.1/dynamic-firewall.dll"
