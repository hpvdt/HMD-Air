#!/usr/bin/env bash

CRDIR="$(
  cd "$(dirname "$0")" || exit
  pwd
)"

FWDIR="$(
  cd "$(dirname "$0")"/.. || exit
  pwd
)"

declare -A origin

# shellcheck disable=SC1090
source "$CRDIR/$1/origin.sh"

for K in "${!origin[@]}"; do
  V="${origin[$K]}"
  File="$FWDIR/message_definitions/$K.xml"

  echo "$V -> $File"

  python -c "import urllib.request; print(urllib.request.urlopen('$V').read().decode('utf-8') )" > \
   "$File"
done

