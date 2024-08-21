#!/usr/bin/env bash

CRDIR="$(
  cd "$(dirname "$0")" || exit
  pwd
)"

# shellcheck disable=SC1090
source "$CRDIR/$1/compose.sh"

FWDIR="$(
  cd "$(dirname "$0")"/.. || exit
  pwd
)"

cd "$FWDIR" || exit

#languages=("Python" "C" "CS")
languages=("CS")

for k in "${languages[@]}"; do

  declare -a paths

  # shellcheck disable=SC2154
  for kk in "${elements[@]}"; do
    paths+=("$FWDIR/message_definitions/${kk}.xml")
  done

mkdir -p Scripts

  mavgen.py --lang="$k" --wire-protocol=2.0 \
    --output="Scripts" "${paths[@]}"
done
