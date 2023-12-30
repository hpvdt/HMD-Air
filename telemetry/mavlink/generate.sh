#!/usr/bin/env bash

CRDIR="$(
  cd "$(dirname "$0")" || exit
  pwd
)"

cd "$CRDIR" || exit

mkdir -p __generated

dialects=("Python" "C" "CS")

for i in "${dialects[@]}"; do
  python -m pymavlink.tools.mavgen --lang="$i" --wire-protocol=2.0 \
    --output="__generated/HPVDT_falcon/$i" "message_definitions/hpvdt/falcon_sensors.xml"
done
