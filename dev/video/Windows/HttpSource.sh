
/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO ORBIT360 4K" :dshow-adev=none :dshow-size="3840x1920" \
  --sout "#transcode{vcodec=h264,venc=x264{preset=ultrafast,tune=zerolatency,intra-refresh,lookahead=10,keyint=15},vb=9999,scale=auto,acodec=none,scode=none}:http{dst=localhost:8080/test.wmv}" --no-audio --no-sout-all #--sout-keep

#/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv dshow:// :dshow-vdev="PIXPRO SP360 4K" :dshow-adev=none :dshow-size="1920x1080" \
#  --sout "#transcode{vcodec=h264,venc=x264{preset=ultrafast,tune=zerolatency,intra-refresh,lookahead=10,keyint=15},vb=9999,scale=auto,acodec=none,scode=none}:http{dst=localhost:8080/test.wmv}" --no-audio --no-sout-all #--sout-keep



#cvlc --miface=$ETH v4l2:///dev/video0 :input-slave=alsa://hw:0,0 :sout=#transcode{vcodec=h264,venc=x264{preset=ultrafast,tune=zerolatency,intra-refresh,lookahead=10,keyint=15},scale=auto,acodec=mpga,ab=128}:rtp{dst=224.10.0.1,port=5004,mux=ts} :sout-keep >/dev/null 2>/dev/null &
