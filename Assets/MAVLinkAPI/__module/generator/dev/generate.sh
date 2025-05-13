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


declare -A languages

languages["CS"]="Scripts"
languages["Python"]="src/main/python/mavlink"
#languages["Java"]="src/main/java/com/MAVLink"

for K in "${!languages[@]}"; do

  declare -a paths

  # shellcheck disable=SC2154
  for kk in "${elements[@]}"; do
    paths+=("$FWDIR/message_definitions/${kk}.xml")
  done

  target_dir="${FWDIR}/${languages[$K]}"

# disabled for safety
#  rm -r "${target_dir}*"
#  find . -name '${target_dir}*' -delete
#  mkdir -p "$target_dir"

  mavgen.py --lang="$K" --wire-protocol=2.0 \
    --output="$target_dir" "${paths[@]}"
done
