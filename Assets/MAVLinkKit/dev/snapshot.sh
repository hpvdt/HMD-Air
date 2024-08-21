#!/usr/bin/env bash

mkdir -p __snapshot

# conda (won't work if not inside conda env)
conda env export > __snapshot/conda-env-full.yml

