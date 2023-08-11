
#CAMERA="PIXPRO ORBIT360 4K"
#SIZE="3840x1920"

CAMERA="USB2.0 HD UVC WebCam"
SIZE="640x480"

HTTP="#http{dst=localhost:8080/test.wmv}"
RTSP="#rtp{sdp=rtsp://:8090/test.wmv}"
UDP="#udp{dst=localhost:8090}"

CODEC="venc=x264{preset=veryfast,tune=zerolatency,intra-refresh,sync-lookahead=0,rc-lookahead=0,keyint=25},ab=128"

TRANSCODE="#transcode{$CODEC,scale=auto,acode=none,scode=none}"

#SOUT="$TRANSCODE:$HTTP"
#SOUT="$TRANSCODE:$RTSP""
SOUT="$UDP"

/c/Program\ Files/VideoLAN/VLC/vlc.exe -vvv \
  dshow:// :dshow-vdev="$CAMERA" :dshow-adev=none :dshow-sdev=none :dshow-size="$SIZE" \
  --sout "${SOUT}" \
  --no-audio --sout-keep
