
/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO ORBIT360 4K" :dshow-adev=none :dshow-size="3840x1920" \
  --sout "#transcode{vcodec=h264,acodec=none}:http{dst=localhost:8080/test.wmv}" --no-audio --no-sout-all #--sout-keep
