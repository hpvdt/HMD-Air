#!/usr/bin/env bash

declare -A origin

origin["ardupilotmega"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/ardupilotmega.xml"
origin["common"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/common.xml"
origin["minimal"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/minimal.xml"
origin["uAvionix"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/uAvionix.xml"
origin["icarous"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/icarous.xml"
origin["loweheiser"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/loweheiser.xml"
origin["cubepilot"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/cubepilot.xml"
# shellcheck disable=SC2034
origin["csAirLink"]="https://github.com/ArduPilot/mavlink/raw/master/message_definitions/v1.0/csAirLink.xml"
