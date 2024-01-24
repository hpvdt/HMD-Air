#!/usr/bin/env bash

CRDIR="$(
  cd "$(dirname "$0")" || exit
  pwd
)"

cd "$CRDIR" || exit

python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 --output="__generated/csharp" "message_definitions/common.xml"

python -m pymavlink.tools.mavgen --lang=Python --wire-protocol=2.0 --output="__generated/python" "message_definitions/common.xml"

python -m pymavlink.tools.mavgen --lang=C --wire-protocol=2.0 --output="__generated/C" "message_definitions/common.xml"

#python -m pymavlink.tools.mavgen --lang=C --wire-protocol=2.0 --output="__generated/c" "message_definitions/ardupilotmega.xml" "message_definitions/offspec.xml"
