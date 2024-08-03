#!/usr/bin/env bash

CRDIR="$(
  cd "$(dirname "$0")" || exit
  pwd
)"

cd "$CRDIR" || exit

mkdir -p __generated

dialects=("Python" "C" "CS")
#dialects=("CS")

for i in "${dialects[@]}"; do
  mavgen.py --lang="$i" --wire-protocol=2.0 \
    --output="__generated/HPVDT_falcon/$i" "message_definitions/hpvdt/falcon_sensors.xml"
done
