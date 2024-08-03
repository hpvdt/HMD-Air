#!/usr/bin/env bash

mkdir -p __snapshot

conda env export --from-history --no-builds > __snapshot/conda-env.yml
conda env export > __snapshot/conda-env-full.yml
