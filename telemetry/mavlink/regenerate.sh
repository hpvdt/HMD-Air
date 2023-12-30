#!/usr/bin/env bash

python -m pymavlink.tools.mavgen --lang=CS --wire-protocol=2.0 --output="__generated/cs" "message_definitions/ardupilotmega.xml" "message_definitions/offspec.xml"

#python -m pymavlink.tools.mavgen --lang=WLua --wire-protocol=2.0 "message_definitions/ardupilotmega.xml" "message_definitions/offspec.xml"
