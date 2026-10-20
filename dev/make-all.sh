#!/usr/bin/env bash

#CRDIR="$(cd "`dirname "$0"`"; pwd)"
FWDIR="$(cd "`dirname "$0"`"/..; pwd)"

# ar-drivers-rs was promoted to a full package (Packages/ARGlassesSDK, gitmodule),
# the driver now builds from its embedded crate and installs into the package Plugins folder
exec "$FWDIR/Packages/ARGlassesSDK/dev/make-all.sh"
