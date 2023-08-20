#/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO ORBIT360 4K" :dshow-adev=none :dshow-size="3840x1920" \
#  --sout "#transcode{vcodec=h264,acodec=none,scodec=none}:rtp{sdp=rtsp://:8554/}" --no-audio --no-sout-all #--sout-keep

#/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO SP360 4K" :dshow-adev=none :dshow-size="1920x1080" \
#  --sout "#transcode{vcodec=h264,acodec=none,scodec=none}:rtp{sdp=rtsp://:8554/}" --no-audio --no-sout-all #--sout-keep
  
/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO SP360 4K" :dshow-adev=none :dshow-size="1920x1080" \
  --sout "#transcode{vcodec=h264,acodec=none,vb=9999,fps=30}:rtp{sdp=rtsp://:8554/}" --no-audio --no-sout-all #--sout-keep
  
#transcode{vcodec=VP80,vb=4000,scale=Auto,acodec=none,scodec=none}