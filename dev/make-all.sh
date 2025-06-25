#!/usr/bin/env bash

#CRDIR="$(cd "`dirname "$0"`"; pwd)"
FWDIR="$(cd "`dirname "$0"`"/..; pwd)"

cd $FWDIR/ar-drivers-rs && \
cargo build --release

TARGET="$FWDIR/Assets/AirAPI/Plugins/libar_drivers.so"
cp $FWDIR/ar-drivers-rs/target/release/libar_drivers.* $TARGET && \
echo "Copied to $TARGET"

source "$FWDIR/ar-drivers-rs/before.sh"

