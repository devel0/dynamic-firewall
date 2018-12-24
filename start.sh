#!/bin/bash

executing_dir()
{
	dirname `readlink -f "$0"`
}

exdir="$(executing_dir)"

/usr/bin/dotnet "$exdir/dynamic-firwall/bin/Debug/netcoreapp2.0/dynamic-firewall.dll"
