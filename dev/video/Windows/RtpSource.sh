
/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO ORBIT360 4K" :dshow-adev=none :dshow-size="3840x1920" \
  --sout "#transcode{vcodec=h264,scale=Auto,acodec=none,scodec=none}" --no-audio --no-sout-all #--sout-keep
  
#/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO ORBIT360 4K" :dshow-adev=none :dshow-size="3840x1920" \
#  --sout "#transcode{vcodec=h264,scale=Auto,acodec=none,scodec=none}:rtp{dst=@,port=5004,mux=ts}" --no-audio --no-sout-all #--sout-keep