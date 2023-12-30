#!/usr/bin/env bash

python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 --output="__generated/csharp" "message_definitions/ardupilotmega.xml" "message_definitions/offspec.xml"

python -m pymavlink.tools.mavgen --lang=Python --wire-protocol=2.0 --output="__generated/python" "message_definitions/ardupilotmega.xml" "message_definitions/offspec.xml"

#python -m pymavlink.tools.mavgen --lang=C --wire-protocol=2.0 --output="__generated/c" "message_definitions/ardupilotmega.xml" "message_definitions/offspec.xml"
